// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.Entities.Comments;
using ExpertBridge.Api.Core.Interfaces.Services;
using ExpertBridge.Api.Data.DatabaseContexts;
using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Queries;
using ExpertBridge.Api.Requests.CreateComment;
using ExpertBridge.Api.Requests.DeleteFileFromComment;
using ExpertBridge.Api.Requests.EditComment;
using ExpertBridge.Api.Requests.ReportComment;
using ExpertBridge.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentsController(
    ExpertBridgeDbContext _dbContext,
    AuthorizationHelper _authHelper
    ) : ControllerBase
{
    // TODO: Create a comment response with CommentQueries
    [HttpPost]
    public async Task<CommentResponse> Create([FromBody] CreateCommentRequest request)
    {
        // TODO: Validate all requests that they contain the required fields.
        // Else, return BadRequest

        ArgumentNullException.ThrowIfNull(request);

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

        var comment = new Comment
        {
            AuthorId = user.Profile.Id,
            Author = profile,
            Content = request.Content,
            ParentCommentId = request.ParentCommentId,
            Post = post,
            PostId = post.Id
        };

        await _dbContext.Comments.AddAsync(comment);
        await _dbContext.SaveChangesAsync();

        var response = await _dbContext.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(profile.Id)
            .FirstAsync();

        return response;
    }

    // CONSIDER!
    // Why not use query params instead of this confusing routing
    // currently used?
    // Something like: /api/comments?postId=aabb33cc

    [AllowAnonymous]
    [Route("api/posts/{postId}/[controller]")]
    [HttpGet] // api/posts/postid/comments
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
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(userProfileId)
            .ToListAsync();

        return comments;
    }

    [HttpGet("{commentId}")]
    public async Task<CommentResponse> Get([FromRoute] string commentId)
    {
        throw new NotImplementedException();
    }

    [Route("api/users/{userId}/[controller]")]
    [HttpGet]
    public async Task<IEnumerable<CommentResponse>> GetAllByUserId([FromRoute] string userId)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{commentId}")]
    public async Task<IActionResult> Patch([FromBody] PatchCommentRequest request)
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

    //[HttpPost("{commnetId}/upvote")]
    //public async Task<IActionResult> UpVote()
    //{
    //    throw new NotImplementedException();
    //}

    //[HttpPost("down-vote/{commnetId}")]
    //public async Task<IActionResult> DownVote()
    //{
    //    throw new NotImplementedException();
    //}

    //[HttpPost("attach/{commentId}")]
    //public async Task<AttachFileToCommentResponse> AttachFile(IFormFile file, [FromRoute] string commentId)
    //{
    //    throw new NotImplementedException();
    //}

    //[HttpDelete("delete-file")]
    //public async Task<IActionResult> DeleteFile([FromBody] DeleteFileFromCommentRequest deleteFileFromCommentRequest)
    //{
    //    throw new NotImplementedException();
    //}

    [HttpPut("edit")]
    public async Task<CommentResponse> Edit([FromBody] EditCommentRequest editCommentRequest)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        throw new NotImplementedException();
    }

    //[HttpPost("report")]
    //public async Task<ReportResponse> Report([FromBody] ReportCommentRequest reportCommentRequest)
    //{
    //    throw new NotImplementedException();
    //}
}
