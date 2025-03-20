// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.DTOs.Requests.CreatePost;
using ExpertBridge.Api.Core.DTOs.Responses;
using ExpertBridge.Api.Core.Entities.Posts;
using ExpertBridge.Api.Core.Interfaces.Repositories;
using ExpertBridge.Api.Core.Interfaces.Services;
using FluentValidation;

namespace ExpertBridge.Api.Application.Services;

public class PostsService(
    IEntityRepository<Post> postRepository,
    IValidator<CreatePostRequest> createPostRequestValidator)
    : IPostsService
{
    public async Task<PostResponse> GetByIdAsync(string id)
    {
        var post = await postRepository.GetByIdAsNoTrackingAsync(id);
        if (post is null) throw new PostNotFoundException("Post not found");
        return new PostResponse(post);
    }
}
