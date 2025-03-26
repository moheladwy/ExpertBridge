// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.Entities.Posts;
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

        return await _dbContext.Posts
            .FullyPopulatedPostQuery()
            .Where(p => p.Id == post.Id)
            .SelectPostResponseFromFullPost(userProfileId)
            .FirstAsync();
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
