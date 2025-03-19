// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.DTOs.Requests.CreateComment;
using ExpertBridge.Api.Core.DTOs.Requests.DeleteFileFromComment;
using ExpertBridge.Api.Core.DTOs.Requests.EditComment;
using ExpertBridge.Api.Core.DTOs.Requests.ReportComment;
using ExpertBridge.Api.Core.DTOs.Responses;
using ExpertBridge.Api.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentsController(ICommentService service) : ControllerBase
{
    [HttpGet("get/{commentId}")]
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

    [HttpPost("create")]
    public async Task<CommentResponse> Create([FromBody] CreateCommentRequest createCommentRequest)
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

    [HttpPost("attach/{commentId}")]
    public async Task<AttachFileToCommentResponse> AttachFile(IFormFile file, [FromRoute] string commentId)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("delete-file")]
    public async Task<IActionResult> DeleteFile([FromBody] DeleteFileFromCommentRequest deleteFileFromCommentRequest)
    {
        throw new NotImplementedException();
    }

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

    [HttpPost("report")]
    public async Task<ReportResponse> Report([FromBody] ReportCommentRequest reportCommentRequest)
    {
        throw new NotImplementedException();
    }
}
