// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.Entities.Posts;
using ExpertBridge.Api.Core.Entities.PostVotes;
using ExpertBridge.Api.Core.Entities.Users;
using ExpertBridge.Api.Core.Interfaces.Services;
using ExpertBridge.Api.Data.DatabaseContexts;
using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Queries;
using ExpertBridge.Api.Requests.CreatePost;
using ExpertBridge.Api.Requests.DeleteFileFromPost;
using ExpertBridge.Api.Requests.EditPost;
using ExpertBridge.Api.Requests.ReportPost;
using ExpertBridge.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController(
    ExpertBridgeDbContext _dbContext,
    AuthorizationHelper _authHelper
    ) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{postId}")]
    public async Task<PostResponse> GetById([FromRoute] string postId)
    {
        //ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId));
        var user = await _authHelper.GetCurrentUserAsync(User);
        var userProfileId = user?.Profile?.Id;

        var post = await _dbContext.Posts
            .FullyPopulatedPostQuery()
            .Where(p => p.Id == postId)
            .SelectPostResponseFromFullPost(userProfileId)
            .FirstOrDefaultAsync();

        return
            post
            ??
            throw new PostNotFoundException($"Post with id={postId} was not found");
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<List<PostResponse>> GetAll()
    {
        Log.Information($"User from HTTP Context: {HttpContext.User.FindFirstValue(ClaimTypes.Email)}");

        var user = await _authHelper.GetCurrentUserAsync(User);
        var userProfileId = user?.Profile?.Id ?? string.Empty;

        return await _dbContext.Posts
            .FullyPopulatedPostQuery()
            .SelectPostResponseFromFullPost(userProfileId)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<PostResponse> Create([FromBody] CreatePostRequest request)
    {
        var user = await _authHelper.GetCurrentUserAsync(User);
        var userProfileId = user?.Profile?.Id ?? string.Empty;

        if (string.IsNullOrEmpty(userProfileId))
        {
            throw new UnauthorizedException();
        }

        if (string.IsNullOrEmpty(request?.Title) || string.IsNullOrEmpty(request?.Content))
        {
            throw new BadHttpRequestException("Title and Content are required");
        }

        var post = new Post
        {
            Title = request.Title,
            Content = request.Content,
            AuthorId = userProfileId
        };

        await _dbContext.Posts.AddAsync(post);
        await _dbContext.SaveChangesAsync();

        return post.SelectPostResponseFromFullPost(userProfileId);
        //return await _dbContext.Posts
        //    .FullyPopulatedPostQuery()
        //    .Where(p => p.Id == post.Id)
        //    .SelectPostResponseFromFullPost(userProfileId)
        //    .FirstAsync();
    }

    /// <summary>
    ///     UpVote a post. If the user has already upvoted the post, it will remove the vote.
    ///     If the user has downvoted the post, it will remove the downvote and add an upvote.
    ///     If the user has not voted on the post, it will add an upvote.
    /// </summary>
    /// <param name="postId">
    ///     The ID of the post to upvote.
    /// </param>
    /// <exception cref="UnauthorizedException">
    ///     Thrown when the user is not authenticated.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when the postId is null or empty.
    /// </exception>
    /// <exception cref="PostNotFoundException">
    ///    Thrown when the post with the given ID does not exist.
    /// </exception>
    /// <returns>
    ///     The number of upvotes for the post after the operation.
    /// </returns>
    [HttpPatch("{postId}/up-vote")]
    public async Task<int> UpVote([FromRoute] string postId)
    {
        // Get the current user from the HTTP context
        var user = await _authHelper.GetCurrentUserAsync(User);
        var userProfileId = user?.Profile?.Id ?? string.Empty;

        // if the user is not authenticated, throw an exception
        if (string.IsNullOrEmpty(userProfileId))
            throw new UnauthorizedException($"Unauthorized Access from User {User}");

        // Check if the postId is valid
        ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId));

        // Check if the post exists in the database
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null) throw new PostNotFoundException($"Post with id={postId} does not exist!");

        var vote = await _dbContext.PostVotes
                .FirstOrDefaultAsync(v => v.PostId == postId && v.ProfileId == userProfileId);

        if (vote is null)
        {
            // If the vote does not exist, create a new one
            var newVote = new PostVote
            {
                PostId = postId,
                ProfileId = userProfileId,
                IsUpvote = true,
            };
            await _dbContext.PostVotes.AddAsync(newVote);
        }
        else if (!vote.IsUpvote)
        {
            // If the vote exists but is a downvote, update it to an upvote
            vote.IsUpvote = true;
            vote.LastModified = DateTime.UtcNow;
            _dbContext.PostVotes.Update(vote);
        }
        else if (vote.IsUpvote && vote.IsDeleted)
        {
            vote.IsDeleted = false;
            vote.DeletedAt = null;
            vote.LastModified = DateTime.UtcNow;
            _dbContext.PostVotes.Update(vote);
        }
        else
        {
            // If the vote exists and is an upvote, remove it (toggle behavior)
            _dbContext.PostVotes.Remove(vote);
        }

        // Save changes to the database
        await _dbContext.SaveChangesAsync();

        // Return the number of upvotes for the post after the operation
        return await _dbContext.PostVotes
            .Where(v => v.PostId == postId &&
                    v.ProfileId == userProfileId &&
                    v.IsUpvote &&
                    !v.IsDeleted)
            .CountAsync();
    }


    [HttpPatch("{postId}/down-vote")]
    public async Task<int> DownVote([FromRoute] string postId)
    {
        // Get the current user from the HTTP context
        var user = await _authHelper.GetCurrentUserAsync(User);
        var userProfileId = user?.Profile?.Id ?? string.Empty;

        // if the user is not authenticated, throw an exception
        if (string.IsNullOrEmpty(userProfileId))
            throw new UnauthorizedException($"Unauthorized Access from User {User}");

        // Check if the postId is valid
        ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId));

        // Check if the post exists in the database
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null) throw new PostNotFoundException($"Post with id={postId} does not exist!");

        var vote = await _dbContext.PostVotes
                .FirstOrDefaultAsync(v => v.PostId == postId && v.ProfileId == userProfileId);

        if (vote is null)
        {
            // If the vote does not exist, create a new one
            var newVote = new PostVote
            {
                PostId = postId,
                ProfileId = userProfileId,
                IsUpvote = false,
            };
            await _dbContext.PostVotes.AddAsync(newVote);
        }
        else if (vote.IsUpvote)
        {
            // If the vote exists but is a upvote, update it to an downvote
            vote.IsUpvote = false;
            vote.IsDeleted = false;
            vote.DeletedAt = null;
            vote.LastModified = DateTime.UtcNow;

            // NO NEED, because the tracking takes care of this.
            _dbContext.PostVotes.Update(vote);
        }
        else if (!vote.IsUpvote && vote.IsDeleted)
        {
            // If the vote exists but is deleted, unmark it as deleted.
            vote.IsDeleted = false;
            vote.DeletedAt = null;
            vote.LastModified = DateTime.UtcNow;

            // NO NEED, because the tracking takes care of this.
            _dbContext.PostVotes.Update(vote);
        }
        else
        {
            // If the vote exists and is an downvote, remove it (toggle behavior)
            _dbContext.PostVotes.Remove(vote);
        }

        // Save changes to the database
        await _dbContext.SaveChangesAsync();

        // Return the number of downvotes for the post after the operation
        return await _dbContext.PostVotes
            .Where(v => v.PostId == postId &&
                    v.ProfileId == userProfileId &&
                    !v.IsUpvote &&
                    !v.IsDeleted)
            .CountAsync();
    }

    [HttpPut]
    public async Task<PostResponse> Edit([FromBody] EditPostRequest editPostRequest)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{postId}")]
    public async Task<IActionResult> Delete([FromRoute] string postId)
    {
        throw new NotImplementedException();
    }





















    //    [HttpPost("attach/{postId}")]
    //    public async Task<AttachFileToPostResponse> AttachFile(IFormFile file, [FromRoute] string postId)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    [HttpDelete("delete-file")]
    //    public async Task<IActionResult> DeleteFile([FromBody] DeleteFileFromPostRequest deleteFileFromPostRequest)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    [HttpPost("report")]
    //    public async Task<ReportResponse> Report([FromBody] ReportPostRequest reportPostRequest)
    //    {
    //        throw new NotImplementedException();
    //    }
}
