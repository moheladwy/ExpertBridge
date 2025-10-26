// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Radzen;

namespace ExpertBridge.Admin.Components.Pages.MainContent;

public partial class Posts : ComponentBase
{
    private readonly int pageSize = 4;
    private int count;
    private int filteredCount;
    private List<PostResponse>? filteredPosts;
    private List<PostResponse> pagedPosts = [];

    private List<PostResponse> posts = [];

    // Search properties
    private string searchText = string.Empty;

    [Inject] private ExpertBridgeDbContext DbContext { get; set; } = default!;

    [Inject] private HybridCache Cache { get; set; } = default!;

    private int displayedCount
    {
        get { return string.IsNullOrWhiteSpace(searchText) ? count : filteredCount; }
    }

    private string pagingSummaryFormat
    {
        get
        {
            return string.IsNullOrWhiteSpace(searchText)
                ? "Displaying page {0} of {1} <b>(total {2} posts)</b>"
                : "Displaying page {0} of {1} <b>({2} posts found, {3} total)</b>";
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadPostsAsync();
    }

    private async Task LoadPostsAsync()
    {
        posts = await Cache.GetOrCreateAsync(
            "admin:posts:all",
            async cancel => await DbContext.Posts
                .FullyPopulatedPostQuery()
                .OrderByDescending(p => p.CreatedAt)
                .SelectPostResponseFromFullPost(null)
                .ToListAsync(cancel),
            cancellationToken: default
        );

        count = posts.Count;
        UpdatePaged(0, pageSize);
    }

    private void UpdatePaged(int skip, int take)
    {
        var sourceList = string.IsNullOrWhiteSpace(searchText) ? posts : filteredPosts ?? posts;
        pagedPosts = sourceList
            .Skip(skip)
            .Take(take)
            .ToList();
    }

    private void PageChanged(PagerEventArgs args)
    {
        UpdatePaged(args.Skip, args.Top);
    }

    private List<PostResponse> GetFilteredPosts()
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return posts;
        }

        var filtered = posts.Where(p =>
            (p.Id?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (p.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (p.Content?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (p.Author?.Username?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (p.Author?.FirstName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (p.Author?.LastName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (p.Tags?.Any(t =>
                (t.EnglishName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (t.ArabicName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
            ) ?? false)
        ).ToList();

        return filtered;
    }

    private void OnSearchChanged(string value)
    {
        searchText = value;
        filteredPosts = GetFilteredPosts();
        filteredCount = filteredPosts.Count;
        UpdatePaged(0, pageSize);
    }

    private void ClearSearch()
    {
        searchText = string.Empty;
        filteredPosts = null;
        filteredCount = 0;
        UpdatePaged(0, pageSize);
    }
}
