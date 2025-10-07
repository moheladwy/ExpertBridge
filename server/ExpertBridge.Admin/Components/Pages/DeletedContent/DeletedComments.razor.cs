// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Radzen;

namespace ExpertBridge.Admin.Components.Pages.DeletedContent;

public partial class DeletedComments : ComponentBase
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly HybridCache _cache;
    public List<CommentResponse> Comments { get; set; }
    private List<CommentResponse>? pagedComments;
    private int pageSize = 4;
    private bool isLoading = true;

    public DeletedComments(ExpertBridgeDbContext dbContext, HybridCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
        Comments = [];
        pagedComments = [];
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            isLoading = true;
            Comments = await GetDeletedComments();
            UpdatePagedComments(0, pageSize);
        }
        finally
        {
            isLoading = false;
        }
        await base.OnInitializedAsync();
    }

    private void OnPageChanged(PagerEventArgs args)
    {
        UpdatePagedComments(args.Skip, args.Top);
    }

    private void UpdatePagedComments(int skip, int take)
    {
        if (Comments != null)
        {
            pagedComments = Comments.Skip(skip).Take(take).ToList();
        }
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
