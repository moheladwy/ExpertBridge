// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Admin.Components.SubModules;

/// <summary>
///     ProfileCard component displays user/profile information in a card format
///     with profile picture, bio, skills, and interests.
/// </summary>
public partial class ProfileCard : ComponentBase
{
    private readonly ExpertBridgeDbContext _dbContext;

    public ProfileCard(ExpertBridgeDbContext dbContext) => _dbContext = dbContext;

    /// <summary>
    ///     The profile data to be displayed in the component.
    /// </summary>
    [Parameter]
    public ProfileResponse? ProfileData { get; set; }

    /// <summary>
    ///     Whether to automatically load user interests when the component initializes.
    /// </summary>
    [Parameter]
    public bool LoadInterests { get; set; } = true;

    /// <summary>
    ///     The list of user interests (tags) to display. If not provided and LoadInterests is true,
    ///     they will be loaded from the database.
    /// </summary>
    [Parameter]
    public List<string>? UserInterests { get; set; }

    /// <summary>
    ///     Whether to show a loading indicator for interests.
    /// </summary>
    public bool ShowLoadingInterests { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        if (ProfileData != null && LoadInterests && UserInterests == null)
        {
            await LoadUserInterestsAsync();
        }

        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        // Reload interests if profile data changed and LoadInterests is enabled
        if (ProfileData != null && LoadInterests && UserInterests == null)
        {
            await LoadUserInterestsAsync();
        }

        await base.OnParametersSetAsync();
    }

    /// <summary>
    ///     Loads user interests (tags) from the database for the current profile.
    /// </summary>
    private async Task LoadUserInterestsAsync()
    {
        if (ProfileData == null)
        {
            return;
        }

        try
        {
            ShowLoadingInterests = true;
            StateHasChanged();

            UserInterests = await _dbContext.UserInterests
                .AsNoTracking()
                .Where(ui => ui.ProfileId == ProfileData.Id)
                .Include(ui => ui.Tag)
                .Select(ui => ui.Tag.EnglishName)
                .ToListAsync();
        }
        catch (Exception)
        {
            // Silently fail - just don't show interests
            UserInterests = [];
        }
        finally
        {
            ShowLoadingInterests = false;
            StateHasChanged();
        }
    }

    /// <summary>
    ///     Manually refresh user interests from the database.
    /// </summary>
    public async Task RefreshInterestsAsync()
    {
        UserInterests = null;
        await LoadUserInterestsAsync();
    }
}
