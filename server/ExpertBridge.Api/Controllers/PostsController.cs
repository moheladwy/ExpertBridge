// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using ExpertBridge.Api.Core;
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
using Serilog;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController(
    IPostsService service,
    ExpertBridgeDbContext _dbContext
    ) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{postId}")]
    public async Task<Post?> GetById([FromRoute] string postId)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId));
        

        return await _dbContext.Posts
            .Include(p => p.Author)
            .Include(p => p.Comments)
            .Where(post => post.Id == postId)
            .FirstOrDefaultAsync();
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IEnumerable<Post>> GetAll()
    {
        Log.Information($"User from HTTP Context: {HttpContext.User.FindFirstValue(ClaimTypes.Email)}");

        return await _dbContext.Posts
            .Include(p => p.Author)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<Post> Create([FromBody] CreatePostRequest request)
    {
        var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
        var user = await _dbContext.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == userEmail);

        if (string.IsNullOrEmpty(user?.Profile?.Id))
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
            AuthorId = user.Profile.Id
        };

        await _dbContext.Posts.AddAsync(post);
        await _dbContext.SaveChangesAsync();

        return post;
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
