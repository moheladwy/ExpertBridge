// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.Entities.Comments;
using ExpertBridge.Api.Core.Interfaces.Services;
using ExpertBridge.Api.Data.DatabaseContexts;
using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Requests.CreateComment;
using ExpertBridge.Api.Requests.DeleteFileFromComment;
using ExpertBridge.Api.Requests.EditComment;
using ExpertBridge.Api.Requests.ReportComment;
using ExpertBridge.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<Comment> Create([FromBody] CreateCommentRequest request)
    {
        // TODO: Validate all requests that they contain the required fields.
        // Else, return BadRequest

        ArgumentNullException.ThrowIfNull(request);

        var user = await _authHelper.GetCurrentUserAsync(User);

        if (user == null)
        {
            throw new UnauthorizedException();
        }

        var comment = new Comment
        {
            AuthorId = user.Profile.Id,
            Content = request.Content,
            ParentCommentId = request.ParentCommentId,
            PostId = request.PostId,
        };

        await _dbContext.Comments.AddAsync(comment);
        await _dbContext.SaveChangesAsync();

        return comment;
    }

    [HttpGet]
    public async Task<CommentResponse> Get([FromRoute] string commentId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("get-all/{postId}")]
    public async Task<IEnumerable<CommentResponse>> GetAllByPostId([FromRoute] string postId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("get-all-by-user-id/{userId}")]
    public async Task<IEnumerable<CommentResponse>> GetAllByUserId([FromRoute] string userId)
    {
        throw new NotImplementedException();
    }

    [HttpPost("up-vote/{commnetId}")]
    public async Task<IActionResult> UpVote()
    {
        throw new NotImplementedException();
    }

    [HttpPost("down-vote/{commnetId}")]
    public async Task<IActionResult> DownVote()
    {
        throw new NotImplementedException();
    }

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
