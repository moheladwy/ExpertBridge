// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.Entities.Comments;
using ExpertBridge.Api.Core.Entities.CommentVotes;
using ExpertBridge.Api.Core.Entities.Posts;
using ExpertBridge.Api.Core.Interfaces.Services;
using ExpertBridge.Api.Data.DatabaseContexts;
using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Models;
using ExpertBridge.Api.Queries;
using ExpertBridge.Api.Requests.CreateComment;
using ExpertBridge.Api.Requests.DeleteFileFromComment;
using ExpertBridge.Api.Requests.EditComment;
using ExpertBridge.Api.Requests.ReportComment;
using ExpertBridge.Api.Responses;
using ExpertBridge.Api.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[ResponseCache(CacheProfileName = CacheProfiles.PersonalizedContent, Duration = 60)]
public class CommentsController(
    ExpertBridgeDbContext _dbContext,
    AuthorizationHelper _authHelper
    ) : ControllerBase
{
    [Route("/api/[controller]")]
    [HttpPost]
    public async Task<CommentResponse> Create([FromBody] CreateCommentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrEmpty(request.Content, nameof(request.Content));

        var user = await _authHelper.GetCurrentUserAsync(User);

        if (user == null)
        {
            throw new UnauthorizedException();
        }

        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId);
        var profile = await _dbContext.Profiles.FirstOrDefaultAsync(p => p.Id == user.Profile.Id);

        // That should be a bad request response.
        if (post == null || profile == null)
        {
            throw new PostNotFoundException($"Post with id={request.PostId} was not found");
        }

        Comment? parentComment = null;
        if (!string.IsNullOrEmpty(request.ParentCommentId))
        {
            parentComment = await _dbContext.Comments
                .FirstOrDefaultAsync(c => c.Id == request.ParentCommentId);
            if (parentComment == null)
            {
                throw new CommentNotFoundException($"Parent comment with id={request.ParentCommentId} was not found");
            }
        }

        var comment = new Comment
        {
            AuthorId = user.Profile.Id,
            Author = profile,
            Content = request.Content,
            ParentCommentId = request.ParentCommentId,
            ParentComment = parentComment,
            Post = post,
            PostId = post.Id
        };

        await _dbContext.Comments.AddAsync(comment);
        await _dbContext.SaveChangesAsync();

        return comment.SelectCommentResponseFromFullComment(profile.Id);
    }

    // CONSIDER!
    // Why not use query params instead of this confusing routing
    // currently used?
    // Something like: /api/comments?postId=aabb33cc

    // THINK!
    // Will migrating to query params make it that more than
    // one endpoint match a certain request? Hmmmm...
    // Ex. /api/comments?postId=aabb33cc /api/comments?userId=bbffcc11


    [Route("/api/posts/{postId}/comments")]
    [AllowAnonymous]
    [HttpGet] // api/posts/<postId>/comments
    public async Task<List<CommentResponse>> GetAllByPostId([FromRoute] string postId)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId));

        var postExists = _dbContext.Posts.Any(p => p.Id == postId);

        if (!postExists)
        {
            throw new PostNotFoundException($"Post with id={postId} was not found");
        }

        var user = await _authHelper.GetCurrentUserAsync(User);
        var userProfileId = user?.Profile?.Id ?? string.Empty;

        var comments = await _dbContext.Comments
            .FullyPopulatedCommentQuery(c => c.PostId == postId)
            .SelectCommentResponseFromFullComment(userProfileId)
            .ToListAsync();

        return comments;
    }

    [HttpGet("{commentId}")]
    public async Task<CommentResponse> Get([FromRoute] string commentId)
    {
        ArgumentException.ThrowIfNullOrEmpty(commentId, nameof(commentId));

        var user = await _authHelper.GetCurrentUserAsync(User);
        var userProfileId = user?.Profile?.Id ?? string.Empty;

        var comment = await _dbContext.Comments
            .FullyPopulatedCommentQuery(c => c.Id == commentId)
            .SelectCommentResponseFromFullComment(userProfileId)
            .FirstOrDefaultAsync();

        if (comment == null)
        {
            throw new CommentNotFoundException($"No comment was found with id={commentId}.");
        }

        return comment;
    }

    [Route("/api/profiles/{profileId}/[controller]")]
    [HttpGet]
    [AllowAnonymous]
    public async Task<List<CommentResponse>> GetAllByUserId([FromRoute] string profileId)
    {
        ArgumentException.ThrowIfNullOrEmpty(profileId, nameof(profileId));
        var user = await _dbContext.Profiles
            .AsNoTracking()
            .FirstOrDefaultAsync(profile => profile.Id == profileId);
        if (user is null) throw new UserNotFoundException($"Profile with id={profileId} was not found");

        return await _dbContext.Comments
            .FullyPopulatedCommentQuery(c => c.AuthorId == profileId)
            .SelectCommentResponseFromFullComment(profileId)
            .ToListAsync();
    }

    [HttpPatch("{commentId}")]
    public async Task<IActionResult> Patch([FromRoute] string commentId, [FromBody] PatchCommentRequest request)
    {
        throw new NotImplementedException();


        // CONSIDER!
        // This approach will make the patch action(endpoint) responsible for
        // too many things that are kind of unrelated to each other.
        // This will make it harder on the RTK side to know what to do
        // before each patch request (due to optimistic UI updates).

        var upvote = request.Upvote.GetValueOrDefault();
        var downvote = request.Downvote.GetValueOrDefault();
        if (upvote || downvote)
        {
            // go to a voting service that takes care of this
        }

        // comment.Content = request.Content ?? comment.Content

        // return content
    }

    [HttpPatch("{commentId}/upvote")]
    public async Task<CommentResponse> Upvote([FromRoute] string commentId)
    {
        ArgumentException.ThrowIfNullOrEmpty(commentId);

        var user = await _authHelper.GetCurrentUserAsync(User);
        var userProfileId = user?.Profile.Id ?? string.Empty;
        var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

        if (user == null || string.IsNullOrEmpty(userProfileId))
        {
            throw new UnauthorizedException();
        }
        if (comment == null)
        {
            throw new CommentNotFoundException($"Comment with id={commentId} was not found");
        }

        var vote = await _dbContext.CommentVotes
            .FirstOrDefaultAsync(v => v.CommentId == commentId && v.ProfileId == userProfileId);

        if (vote == null)
        {
            vote = new CommentVote
            {
                ProfileId = userProfileId,
                CommentId = comment.Id,
                IsUpvote = true
            };

            await _dbContext.AddAsync(vote);
        }
        else
        {
            if (vote.IsUpvote)
            {
                _dbContext.CommentVotes.Remove(vote);
            }
            else
            {
                vote.IsUpvote = true;
                vote.LastModified = DateTime.UtcNow;
            }
        }

        await _dbContext.SaveChangesAsync();

        return await _dbContext.Comments
            .FullyPopulatedCommentQuery(c => c.Id == comment.Id)
            .SelectCommentResponseFromFullComment(user.Profile.Id)
            .FirstAsync();
    }

    [HttpPatch("{commentId}/downvote")]
    public async Task<CommentResponse> Downvote([FromRoute] string commentId)
    {
        ArgumentException.ThrowIfNullOrEmpty(commentId);

        var user = await _authHelper.GetCurrentUserAsync(User);
        var userProfileId = user?.Profile.Id ?? string.Empty;
        var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

        if (user == null || string.IsNullOrEmpty(userProfileId))
        {
            throw new UnauthorizedException();
        }
        if (comment == null)
        {
            throw new CommentNotFoundException($"Comment with id={commentId} was not found");
        }

        var vote = await _dbContext.CommentVotes
            .FirstOrDefaultAsync(v => v.CommentId == commentId && v.ProfileId == userProfileId);

        if (vote == null)
        {
            vote = new CommentVote
            {
                ProfileId = userProfileId,
                CommentId = comment.Id,
                IsUpvote = false,
            };

            await _dbContext.AddAsync(vote);
        }
        else
        {
            if (vote.IsUpvote)
            {
                vote.IsUpvote = false;
                vote.LastModified = DateTime.UtcNow;
            }
            else
            {
                _dbContext.CommentVotes.Remove(vote);
            }
        }

        await _dbContext.SaveChangesAsync();

        return await _dbContext.Comments
            .FullyPopulatedCommentQuery(c => c.Id == comment.Id)
            .SelectCommentResponseFromFullComment(user.Profile.Id)
            .FirstAsync();
    }

    [HttpPatch("{commentId}")]
    public async Task<CommentResponse> Edit([FromRoute] string commentId, [FromBody] EditCommentRequest request)
    {
        // Check if the request is not null
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentException.ThrowIfNullOrEmpty(commentId, nameof(commentId));

        // Check if the user is authorized to edit the comment
        var user = await _authHelper.GetCurrentUserAsync(User);
        if (user is null)
        {
            throw new UnauthorizedException();
        }

        var userProfileId = user.Profile?.Id ?? string.Empty;

        // Check if the comment exists and belongs to the user
        var comment = await _dbContext.Comments
            .FirstOrDefaultAsync(c => c.Id == commentId && c.AuthorId == userProfileId);

        if (comment == null)
        {
            throw new CommentNotFoundException(
                $"Comment with id={commentId} was not found");
        }

        // Update the comment content if provided
        if (!string.IsNullOrEmpty(request.Content))
        {
            comment.Content = request.Content;
            comment.LastModified = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        // Return the updated comment
        return comment.SelectCommentResponseFromFullComment(userProfileId);
    }

    [HttpDelete("{commentId}")]
    public async Task<IActionResult> Delete([FromRoute] string commentId)
    {
        // Check if the id is not null or empty
        ArgumentException.ThrowIfNullOrEmpty(commentId, nameof(commentId));

        // Check if the user is authorized to delete the comment
        var user = await _authHelper.GetCurrentUserAsync(User);
        var userProfileId = user?.Profile.Id ?? string.Empty;

        // Check if the comment exists and belongs to the user
        var comment = await _dbContext.Comments
            .FirstOrDefaultAsync(c => c.Id == commentId && c.AuthorId == userProfileId);

        if (comment != null)
        {
            //builder.HasMany(c => c.Replies)
            //    .WithOne(c => c.ParentComment)
            //    .OnDelete(DeleteBehavior.Cascade);

            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync();
        }

        // BEWARE!
        // In an HTTP DELETE, you always want to return 204 no content.
        // No matter what happens. The only exception is if the auth middleware
        // refused the request from the beginning, else you do not return anything
        // other than no content.
        // https://stackoverflow.com/questions/6439416/status-code-when-deleting-a-resource-using-http-delete-for-the-second-time#comment33002038_6440374
        return NoContent();
    }
}
