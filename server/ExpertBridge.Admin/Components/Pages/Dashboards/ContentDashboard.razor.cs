// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Admin.Components.Pages.Dashboards;

public partial class ContentDashboard : ComponentBase
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly bool _showDataLabels;
    private readonly ContentStats _stats;
    private List<ChartDataItem> _commentChartData;
    private List<ChartDataItem> _jobPostingChartData;
    private bool _loading;
    private List<OverviewChartData> _overviewChartData;
    private List<ChartDataItem> _postChartData;

    public ContentDashboard(ExpertBridgeDbContext dbContext)
    {
        _dbContext = dbContext;
        _loading = true;
        _showDataLabels = true;
        _stats = new ContentStats();
        _postChartData = [];
        _commentChartData = [];
        _jobPostingChartData = [];
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
            await LoadContentStatsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading content data: {ex.Message}");
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task LoadContentStatsAsync()
    {
        // Load Posts statistics
        _stats.ActivePosts = await _dbContext.Posts
            .AsNoTracking()
            .CountAsync();
        _stats.DeletedPosts = await _dbContext.Posts
            .AsNoTracking()
            .IgnoreQueryFilters()
            .CountAsync(p => p.IsDeleted);
        _stats.TotalPosts = _stats.ActivePosts + _stats.DeletedPosts;

        // Load Comments statistics
        _stats.ActiveComments = await _dbContext.Comments
            .AsNoTracking()
            .CountAsync();
        _stats.DeletedComments = await _dbContext.Comments
            .AsNoTracking()
            .IgnoreQueryFilters()
            .CountAsync(c => c.IsDeleted);
        _stats.TotalComments = _stats.ActiveComments + _stats.DeletedComments;

        // Load Job Postings statistics
        _stats.ActiveJobPostings = await _dbContext.JobPostings
            .AsNoTracking()
            .CountAsync();
        _stats.DeletedJobPostings = await _dbContext.JobPostings
            .AsNoTracking()
            .IgnoreQueryFilters()
            .CountAsync(jp => jp.IsDeleted);
        _stats.TotalJobPostings = _stats.ActiveJobPostings + _stats.DeletedJobPostings;

        // Populate chart data
        _postChartData =
        [
            new ChartDataItem { Category = "Active", Value = _stats.ActivePosts },
            new ChartDataItem { Category = "Deleted", Value = _stats.DeletedPosts }
        ];

        _commentChartData =
        [
            new ChartDataItem { Category = "Active", Value = _stats.ActiveComments },
            new ChartDataItem { Category = "Deleted", Value = _stats.DeletedComments }
        ];

        _jobPostingChartData =
        [
            new ChartDataItem { Category = "Active", Value = _stats.ActiveJobPostings },
            new ChartDataItem { Category = "Deleted", Value = _stats.DeletedJobPostings }
        ];

        _overviewChartData =
        [
            new OverviewChartData { Category = "Posts", Active = _stats.ActivePosts, Deleted = _stats.DeletedPosts },
            new OverviewChartData
            {
                Category = "Comments", Active = _stats.ActiveComments, Deleted = _stats.DeletedComments
            },
            new OverviewChartData
            {
                Category = "Job Postings", Active = _stats.ActiveJobPostings, Deleted = _stats.DeletedJobPostings
            }
        ];
    }

    private class ContentStats
    {
        public int TotalPosts { get; set; }
        public int ActivePosts { get; set; }
        public int DeletedPosts { get; set; }

        public int TotalComments { get; set; }
        public int ActiveComments { get; set; }
        public int DeletedComments { get; set; }

        public int TotalJobPostings { get; set; }
        public int ActiveJobPostings { get; set; }
        public int DeletedJobPostings { get; set; }
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
