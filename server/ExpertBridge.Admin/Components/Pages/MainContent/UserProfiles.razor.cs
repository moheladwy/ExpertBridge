// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Admin.Components.Pages.MainContent;

/// <summary>
///     UserProfiles page displays all user profiles with pagination support.
/// </summary>
public partial class UserProfiles : ComponentBase
{
    private readonly int[] pageSizeOptions = [3, 6, 9, 12, 18, 24];

    // Pagination properties
    private int currentPage;
    private int filteredCount;
    private List<ProfileResponse> filteredProfiles = [];
    private bool isLoading = true;
    private List<ProfileResponse> pagedProfiles = [];
    private int pageSize = 3; // 3 columns x 3 rows

    private List<ProfileResponse> profiles = [];

    // Search properties
    private string searchText = string.Empty;
    private int totalProfiles;

    public UserProfiles(ExpertBridgeDbContext dbContext)
    {
        DbContext = dbContext;
    }

    private int displayedProfileCount
    {
        get { return string.IsNullOrWhiteSpace(searchText) ? totalProfiles : filteredCount; }
    }

    private int totalPages
    {
        get { return (int)Math.Ceiling((double)displayedProfileCount / pageSize); }
    }

    [Inject] public ExpertBridgeDbContext DbContext { get; init; }

    protected override async Task OnInitializedAsync()
    {
        await LoadProfilesAsync();
        await base.OnInitializedAsync();
    }

    private async Task LoadProfilesAsync()
    {
        try
        {
            isLoading = true;

            // Get total count
            totalProfiles = await DbContext.Profiles
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .CountAsync();

            // Load all profiles
            profiles = await DbContext.Profiles
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.ProfileSkills)
                .ThenInclude(ps => ps.Skill)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Votes)
                .Where(p => !p.IsDeleted)
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
                .ToListAsync();

            UpdatePagedProfiles();
        }
        finally
        {
            isLoading = false;
        }
    }

    private List<ProfileResponse> GetFilteredProfiles()
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return profiles;
        }

        return profiles.Where(p =>
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
        filteredCount = filteredProfiles.Count;
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
        var source = string.IsNullOrWhiteSpace(searchText) ? profiles : filteredProfiles;
        pagedProfiles = source
            .Skip(currentPage * pageSize)
            .Take(pageSize)
            .ToList();
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

    private async Task RefreshData()
    {
        currentPage = 0;
        await LoadProfilesAsync();
    }
}
