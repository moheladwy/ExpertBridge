// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Admin.Components.Pages.Dashboards;

public partial class Home : ComponentBase
{
    private readonly ExpertBridgeDbContext DbContext;
    private bool _loading;
    private bool _showDataLabels;
    private DashboardStats _stats;
    private List<ChartDataItem> _userChartData;
    private List<ChartDataItem> _profileChartData;
    private List<OverviewChartData> _overviewChartData;

    public Home(ExpertBridgeDbContext dbContext)
    {
        DbContext = dbContext;
        _loading = true;
        _showDataLabels = true;
        _stats = new();
        _userChartData = [];
        _profileChartData = [];
        _overviewChartData = [];
    }

    protected override async Task OnInitializedAsync()
    {
        await RefreshData();
    }

    private async Task RefreshData()
    {
        try
        {
            _loading = true;
            await LoadDashboardDataAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading dashboard data: {ex.Message}");
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task LoadDashboardDataAsync()
    {
        // Load user and profile statistics
        await LoadUserStatsAsync();
        await LoadProfileStatsAsync();

        // Populate chart data
        _userChartData =
        [
            new ChartDataItem { Category = "Active", Value = _stats.ActiveUsers },
            new ChartDataItem { Category = "Deleted", Value = _stats.DeletedUsers }
        ];

        _profileChartData =
        [
            new ChartDataItem { Category = "Active", Value = _stats.ActiveProfiles },
            new ChartDataItem { Category = "Deleted", Value = _stats.DeletedProfiles }
        ];

        _overviewChartData =
        [
            new OverviewChartData { Category = "Users", Active = _stats.ActiveUsers, Deleted = _stats.DeletedUsers },
            new OverviewChartData { Category = "Profiles", Active = _stats.ActiveProfiles, Deleted = _stats.DeletedProfiles }
        ];
    }

    private async Task LoadUserStatsAsync()
    {
        _stats.ActiveUsers = await DbContext.Profiles
            .AsNoTracking()
            .CountAsync();
        _stats.DeletedUsers = await DbContext.Profiles
            .AsNoTracking()
            .IgnoreQueryFilters()
            .CountAsync(u => u.IsDeleted);
        _stats.TotalUsers = _stats.ActiveUsers + _stats.DeletedUsers;
    }

    private async Task LoadProfileStatsAsync()
    {
        _stats.ActiveProfiles = await DbContext.Profiles
            .AsNoTracking()
            .CountAsync();
        _stats.DeletedProfiles = await DbContext.Profiles
            .AsNoTracking()
            .IgnoreQueryFilters()
            .CountAsync(p => p.IsDeleted);
        _stats.TotalProfiles = _stats.ActiveProfiles + _stats.DeletedProfiles;
    }

    private class DashboardStats
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int DeletedUsers { get; set; }

        public int TotalProfiles { get; set; }
        public int ActiveProfiles { get; set; }
        public int DeletedProfiles { get; set; }
    }

    private class ChartDataItem
    {
        public string Category { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    private class OverviewChartData
    {
        public string Category { get; set; } = string.Empty;
        public int Active { get; set; }
        public int Deleted { get; set; }
    }
}
