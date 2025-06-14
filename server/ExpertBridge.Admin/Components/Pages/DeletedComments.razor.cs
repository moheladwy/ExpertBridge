// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace ExpertBridge.Admin.Components.Pages;

public partial class DeletedComments : ComponentBase
{
    private readonly ExpertBridgeDbContext _dbContext;
    public List<CommentResponse> Comments { get; set; }
    private readonly HybridCache _cache;

    public DeletedComments(ExpertBridgeDbContext dbContext, HybridCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
        Comments = [];
    }

    protected override async Task OnInitializedAsync()
    {
        Comments = await GetDeletedComments();
        await base.OnInitializedAsync();
    }

    private async Task<List<CommentResponse>> GetDeletedComments()
    {
        const string cacheKey = "deleted-comments";
        var cachedComments = await _cache.GetOrCreateAsync<List<CommentResponse>>(cacheKey, async cancellationToken =>
        {
            var comments = await _dbContext.Comments
                .IgnoreQueryFilters()
                .FullyPopulatedCommentQuery()
                .SelectCommentResponseFromFullComment(null)
                .ToListAsync(cancellationToken);
            return comments;
        });
        return cachedComments;
    }
}
