// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.DTOs.Requests.CreatePost;
using ExpertBridge.Api.Core.DTOs.Requests.DeleteFileFromPost;
using ExpertBridge.Api.Core.DTOs.Requests.EditPost;
using ExpertBridge.Api.Core.DTOs.Requests.ReportPost;
using ExpertBridge.Api.Core.DTOs.Responses;
using ExpertBridge.Api.Core.Entities.Posts;
using ExpertBridge.Api.Core.Interfaces.Services;
using ExpertBridge.Api.Data.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController(
    IPostService service,
    ExpertBridgeDbContext _dbContext
    ) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{postId}")]
    public async Task<PostResponse> GetById([FromRoute] string postId)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId));

        return await _dbContext.Posts
            .Where(post => post.Id == postId)
            .Select(post => new PostResponse(post))
            .FirstAsync();
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IEnumerable<Post>> GetAll()
    {
        return await _dbContext.Posts
            .ToListAsync();
    }

    [HttpPost]
    public async Task<PostResponse> Create([FromBody] CreatePostRequest createPostRequest)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{postId}/up-vote")]
    public async Task<IActionResult> UpVote([FromRoute] string postId)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{postId}/down-vote")]
    public async Task<IActionResult> DownVote([FromRoute] string postId)
    {
        throw new NotImplementedException();
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





















    [HttpPost("attach/{postId}")]
    public async Task<AttachFileToPostResponse> AttachFile(IFormFile file, [FromRoute] string postId)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("delete-file")]
    public async Task<IActionResult> DeleteFile([FromBody] DeleteFileFromPostRequest deleteFileFromPostRequest)
    {
        throw new NotImplementedException();
    }

    [HttpPost("report")]
    public async Task<ReportResponse> Report([FromBody] ReportPostRequest reportPostRequest)
    {
        throw new NotImplementedException();
    }
}
