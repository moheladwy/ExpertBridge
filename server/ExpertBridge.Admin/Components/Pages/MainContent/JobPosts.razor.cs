// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using ExpertBridge.Contract.Queries;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Radzen;

namespace ExpertBridge.Admin.Components.Pages.MainContent;

public partial class JobPosts : ComponentBase
{
    private readonly int pageSize = 4;
    private int count;
    private int filteredCount;
    private List<JobPostingResponse>? filteredJobPosts;

    private List<JobPostingResponse> jobPosts = [];
    private List<JobPostingResponse> pagedJobPosts = [];

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
                ? "Displaying page {0} of {1} <b>(total {2} job posts)</b>"
                : "Displaying page {0} of {1} <b>({2} job posts found, {3} total)</b>";
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadJobPostsAsync();
    }

    private async Task LoadJobPostsAsync()
    {
        jobPosts = await Cache.GetOrCreateAsync(
            "admin:job-posts:all",
            async cancel => await DbContext.JobPostings
                .FullyPopulatedJobPostingQuery()
                .OrderByDescending(jp => jp.CreatedAt)
                .SelectJopPostingResponseFromFullJobPosting(null)
                .ToListAsync(cancel),
            cancellationToken: default
        );

        count = jobPosts.Count;
        UpdatePaged(0, pageSize);
    }

    private void UpdatePaged(int skip, int take)
    {
        var sourceList = string.IsNullOrWhiteSpace(searchText) ? jobPosts : filteredJobPosts ?? jobPosts;
        pagedJobPosts = sourceList
            .Skip(skip)
            .Take(take)
            .ToList();
    }

    private void PageChanged(PagerEventArgs args)
    {
        UpdatePaged(args.Skip, args.Top);
    }

    private List<JobPostingResponse> GetFilteredJobPosts()
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return jobPosts;
        }

        var filtered = jobPosts.Where(jp =>
            (jp.Id?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (jp.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (jp.Content?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (jp.Author?.Username?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (jp.Author?.FirstName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (jp.Author?.LastName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            jp.Budget.ToString(CultureInfo.InvariantCulture).Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
            (jp.Area?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (jp.Tags?.Any(t =>
                (t.EnglishName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (t.ArabicName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
            ) ?? false)
        ).ToList();
        return filtered;
    }

    private void OnSearchChanged(string value)
    {
        searchText = value;
        filteredJobPosts = GetFilteredJobPosts();
        filteredCount = filteredJobPosts.Count;
        UpdatePaged(0, pageSize);
    }

    private void ClearSearch()
    {
        searchText = string.Empty;
        filteredJobPosts = null;
        filteredCount = 0;
        UpdatePaged(0, pageSize);
    }
}
