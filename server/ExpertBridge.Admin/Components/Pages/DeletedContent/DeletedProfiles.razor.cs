// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace ExpertBridge.Admin.Components.Pages.DeletedContent;

/// <summary>
///     DeletedProfiles page displays all soft-deleted user profiles with pagination support.
/// </summary>
public sealed partial class DeletedProfiles : ComponentBase
{
  private readonly HybridCache _cache;
  private readonly ExpertBridgeDbContext _dbContext;

  // Pagination properties
  private readonly int[] pageSizeOptions = [3, 6, 9, 12, 18, 24];
  private int currentPage;
  private int filteredCount;
  private List<ProfileResponse>? filteredProfiles;
  private bool isLoading = true;
  private List<ProfileResponse>? pagedProfiles;
  private int pageSize = 3; // 3 columns x 3 rows

  private List<ProfileResponse>? deletedProfiles;

  // Search properties
  private string searchText = string.Empty;

  public DeletedProfiles(ExpertBridgeDbContext dbContext, HybridCache cache)
  {
    _dbContext = dbContext;
    _cache = cache;
    deletedProfiles = [];
    pagedProfiles = [];
    filteredProfiles = [];
  }

  private int displayedProfileCount
  {
    get { return string.IsNullOrWhiteSpace(searchText) ? deletedProfiles?.Count ?? 0 : filteredCount; }
  }

  private int totalPages
  {
    get { return (int)Math.Ceiling((double)displayedProfileCount / pageSize); }
  }

  protected override async Task OnInitializedAsync()
  {
    try
    {
      isLoading = true;
      deletedProfiles = await GetDeletedProfiles();
      UpdatePagedProfiles();
    }
    finally
    {
      isLoading = false;
    }

    await base.OnInitializedAsync();
  }

  private List<ProfileResponse> GetFilteredProfiles()
  {
    if (deletedProfiles == null || string.IsNullOrWhiteSpace(searchText))
    {
      return deletedProfiles ?? [];
    }

    return deletedProfiles.Where(p =>
        (p.FirstName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
        (p.LastName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
        (p.Username?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
        (p.Email?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
        (p.JobTitle?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
        (p.Bio?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
        (p.Skills?.Any(s => s.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ?? false)
    ).ToList();
  }

  private void OnSearchChanged(string value)
  {
    searchText = value;
    filteredProfiles = GetFilteredProfiles();
    filteredCount = filteredProfiles?.Count ?? 0;
    currentPage = 0; // Reset to first page when search changes
    UpdatePagedProfiles();
  }

  private void ClearSearch()
  {
    searchText = string.Empty;
    OnSearchChanged(string.Empty);
  }

  private void UpdatePagedProfiles()
  {
    var source = string.IsNullOrWhiteSpace(searchText) ? deletedProfiles : filteredProfiles;
    if (source != null)
    {
      pagedProfiles = source
          .Skip(currentPage * pageSize)
          .Take(pageSize)
          .ToList();
    }
  }

  private void OnPageChanged(int pageIndex)
  {
    currentPage = pageIndex;
    UpdatePagedProfiles();
  }

  private void OnPageSizeChanged(object value)
  {
    if (value is int newPageSize)
    {
      pageSize = newPageSize;
      currentPage = 0; // Reset to first page when page size changes
      UpdatePagedProfiles();
    }
  }

  private async Task<List<ProfileResponse>> GetDeletedProfiles()
  {
    const string cacheKey = "deleted-profiles";
    var profiles = await _cache.GetOrCreateAsync<List<ProfileResponse>>(cacheKey, async token =>
        await _dbContext.Profiles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.ProfileSkills)
            .ThenInclude(ps => ps.Skill)
            .Include(p => p.Comments)
            .ThenInclude(c => c.Votes)
            .Where(p => p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProfileResponse
            {
              Id = p.Id,
              UserId = p.UserId,
              CreatedAt = p.CreatedAt!.Value,
              Email = p.Email,
              FirstName = p.FirstName,
              LastName = p.LastName,
              IsBanned = p.IsBanned,
              JobTitle = p.JobTitle,
              Bio = p.Bio,
              PhoneNumber = p.PhoneNumber,
              ProfilePictureUrl = p.ProfilePictureUrl,
              Rating = p.Rating,
              RatingCount = p.RatingCount,
              Username = p.Username,
              IsOnboarded = p.User.IsOnboarded,
              Skills = p.ProfileSkills.Select(ps => ps.Skill.Name).ToList(),
              CommentsUpvotes = p.Comments.Sum(c => c.Votes.Count(v => v.IsUpvote)),
              CommentsDownvotes = p.Comments.Sum(c => c.Votes.Count(v => !v.IsUpvote)),
              Reputation = p.Comments.Sum(c => c.Votes.Count(v => v.IsUpvote)) -
                             p.Comments.Sum(c => c.Votes.Count(v => !v.IsUpvote))
            })
            .ToListAsync(token)
    );
    return profiles;
  }
}
