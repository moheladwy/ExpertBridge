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

public sealed partial class DeletedPosts : ComponentBase
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly HybridCache _cache;
    private List<PostResponse>? reportedPosts;
    private List<PostResponse>? pagedPosts;
    private int pageSize = 4;
    private bool isLoading = true;

    // Search properties
    private string searchText = string.Empty;
    private List<PostResponse>? filteredPosts;
    private int filteredCount;
    private int displayedPostCount => string.IsNullOrWhiteSpace(searchText) ? (reportedPosts?.Count ?? 0) : filteredCount;
    private string pagingSummaryFormat => $"Displaying page {{0}} of {{1}} (total {{2}} {(string.IsNullOrWhiteSpace(searchText) ? "deleted posts" : "results")})";

    public DeletedPosts(ExpertBridgeDbContext dbContext, HybridCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
        reportedPosts = [];
        pagedPosts = [];
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            isLoading = true;
            reportedPosts = await GetDeletedPosts();
            UpdatePagedPosts(0, pageSize);
        }
        finally
        {
            isLoading = false;
        }
        await base.OnInitializedAsync();
    }

    private void OnPageChanged(PagerEventArgs args)
    {
        UpdatePagedPosts(args.Skip, args.Top);
    }

    private List<PostResponse> GetFilteredPosts()
    {
        if (reportedPosts == null || string.IsNullOrWhiteSpace(searchText))
            return reportedPosts ?? [];

        return reportedPosts.Where(p =>
            (p.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (p.Content?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (p.Author?.Username?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (p.Author?.FirstName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (p.Author?.LastName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (p.Tags?.Any(t => t.EnglishName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                             t.ArabicName.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ?? false)
        ).ToList();
    }

    private void OnSearchChanged(string value)
    {
        searchText = value;
        filteredPosts = GetFilteredPosts();
        filteredCount = filteredPosts.Count;
        UpdatePagedPosts(0, pageSize); // Reset to first page when search changes
    }

    private void ClearSearch()
    {
        searchText = string.Empty;
        OnSearchChanged(string.Empty);
    }

    private void UpdatePagedPosts(int skip, int take)
    {
        var source = string.IsNullOrWhiteSpace(searchText) ? reportedPosts : filteredPosts;
        if (source != null)
        {
            pagedPosts = source.Skip(skip).Take(take).ToList();
        }
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
