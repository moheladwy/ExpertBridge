// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace ExpertBridge.Admin.Components.Pages;

public sealed partial class DeletedPosts : ComponentBase
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly HybridCache _cache;
    private List<PostResponse>? reportedPosts;

    public DeletedPosts(ExpertBridgeDbContext dbContext, HybridCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
        reportedPosts = [];
    }

    protected override async Task OnInitializedAsync()
    {
        reportedPosts = await GetDeletedPosts();
        await base.OnInitializedAsync();
    }

    private async Task<List<PostResponse>> GetDeletedPosts()
    {
        const string cacheKey = "deleted-posts";
        var posts = await _cache.GetOrCreateAsync<List<PostResponse>>(cacheKey, async token =>
                await _dbContext.Posts
                .IgnoreQueryFilters()
                .FullyPopulatedPostQuery(p => p.IsDeleted)
                .SelectPostResponseFromFullPost(null)
                .ToListAsync(token)
        );
        return posts;
    }
}
