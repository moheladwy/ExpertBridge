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

namespace ExpertBridge.Admin.Components.Pages.DeletedContent;

public sealed partial class DeletedJobPosts : ComponentBase
{
  private readonly HybridCache _cache;
  private readonly ExpertBridgeDbContext _dbContext;
  private readonly int pageSize = 4;
  private int filteredCount;
  private List<JobPostingResponse>? filteredJobPosts;
  private bool isLoading = true;
  private List<JobPostingResponse>? pagedJobPosts;
  private List<JobPostingResponse>? deletedJobPosts;

  // Search properties
  private string searchText = string.Empty;

  public DeletedJobPosts(ExpertBridgeDbContext dbContext, HybridCache cache)
  {
    _dbContext = dbContext;
    _cache = cache;
    deletedJobPosts = [];
    pagedJobPosts = [];
  }

  private int displayedJobPostCount
  {
    get { return string.IsNullOrWhiteSpace(searchText) ? deletedJobPosts?.Count ?? 0 : filteredCount; }
  }

  private string pagingSummaryFormat
  {
    get
    {
      return
          $"Displaying page {{0}} of {{1}} (total {{2}} {(string.IsNullOrWhiteSpace(searchText) ? "deleted job posts" : "results")})";
    }
  }

  protected override async Task OnInitializedAsync()
  {
    try
    {
      isLoading = true;
      deletedJobPosts = await GetDeletedJobPosts();
      UpdatePagedJobPosts(0, pageSize);
    }
    finally
    {
      isLoading = false;
    }

    await base.OnInitializedAsync();
  }

  private void OnPageChanged(PagerEventArgs args)
  {
    UpdatePagedJobPosts(args.Skip, args.Top);
  }

  private List<JobPostingResponse> GetFilteredJobPosts()
  {
    if (deletedJobPosts == null || string.IsNullOrWhiteSpace(searchText))
    {
      return deletedJobPosts ?? [];
    }

    return deletedJobPosts.Where(jp =>
        (jp.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
        (jp.Content?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
        (jp.Author?.Username?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
        (jp.Author?.FirstName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
        (jp.Author?.LastName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
        jp.Budget.ToString(CultureInfo.InvariantCulture).Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
        (jp.Area?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
        (jp.Tags?.Any(t => t.EnglishName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                          t.ArabicName.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ?? false)
    ).ToList();
  }

  private void OnSearchChanged(string value)
  {
    searchText = value;
    filteredJobPosts = GetFilteredJobPosts();
    filteredCount = filteredJobPosts.Count;
    UpdatePagedJobPosts(0, pageSize); // Reset to first page when search changes
  }

  private void ClearSearch()
  {
    searchText = string.Empty;
    OnSearchChanged(string.Empty);
  }

  private void UpdatePagedJobPosts(int skip, int take)
  {
    var source = string.IsNullOrWhiteSpace(searchText) ? deletedJobPosts : filteredJobPosts;
    if (source != null)
    {
      pagedJobPosts = source.Skip(skip).Take(take).ToList();
    }
  }

  private async Task<List<JobPostingResponse>> GetDeletedJobPosts()
  {
    const string cacheKey = "deleted-job-posts";
    var jobPosts = await _cache.GetOrCreateAsync<List<JobPostingResponse>>(cacheKey, async token =>
        await _dbContext.JobPostings
            .IgnoreQueryFilters()
            .FullyPopulatedJobPostingQuery(jp => jp.IsDeleted)
            .SelectJopPostingResponseFromFullJobPosting(null)
            .ToListAsync(token)
    );
    return jobPosts;
  }
}
