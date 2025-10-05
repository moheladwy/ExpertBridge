// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Admin.Components.Pages;

public partial class Home
{
    private readonly ExpertBridgeDbContext DbContext;
    private bool _loading;
    private bool _showDataLabels;
    private DashboardStats _stats;
    private List<ChartDataItem> _userChartData;
    private List<ChartDataItem> _postChartData;
    private List<ChartDataItem> _commentChartData;
    private List<ChartDataItem> _jobPostingChartData;
    private List<OverviewChartData> _overviewChartData;

    public Home(ExpertBridgeDbContext dbContext)
    {
        DbContext = dbContext;
        _loading = true;
        _showDataLabels = true;
        _stats = new();
        _userChartData = [];
        _postChartData = [];
        _commentChartData = [];
        _jobPostingChartData = [];
        _overviewChartData = [];
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
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
        // Load all statistics in parallel
        await LoadUserStatsAsync();
        await LoadPostStatsAsync();
        await LoadCommentStatsAsync();
        await LoadJobPostingStatsAsync();
        await LoadJobStatsAsync();
        await LoadProfileStatsAsync();
        await LoadJobApplicationStatsAsync();

        // await Task.WhenAll(usersTask, postsTask, commentsTask, jobPostingsTask,
        //     jobsTask, profilesTask, jobApplicationsTask);

        // Populate chart data
        _userChartData =
        [
            new ChartDataItem { Category = "Active", Value = _stats.ActiveUsers },
            new ChartDataItem { Category = "Deleted", Value = _stats.DeletedUsers }
        ];

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
            new OverviewChartData { Category = "Users", Active = _stats.ActiveUsers, Deleted = _stats.DeletedUsers },
            new OverviewChartData { Category = "Posts", Active = _stats.ActivePosts, Deleted = _stats.DeletedPosts },
            new OverviewChartData { Category = "Comments", Active = _stats.ActiveComments, Deleted = _stats.DeletedComments },
            new OverviewChartData { Category = "Job Postings", Active = _stats.ActiveJobPostings, Deleted = _stats.DeletedJobPostings },
            new OverviewChartData { Category = "Jobs", Active = _stats.ActiveJobs, Deleted = _stats.DeletedJobs },
            new OverviewChartData { Category = "Profiles", Active = _stats.ActiveProfiles, Deleted = _stats.DeletedProfiles },
            new OverviewChartData { Category = "Applications", Active = _stats.ActiveJobApplications, Deleted = _stats.DeletedJobApplications }
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

    private async Task LoadPostStatsAsync()
    {
        _stats.ActivePosts = await DbContext.Posts
            .AsNoTracking()
            .CountAsync();
        _stats.DeletedPosts = await DbContext.Posts
            .AsNoTracking()
            .IgnoreQueryFilters()
            .CountAsync(p => p.IsDeleted);
        _stats.TotalPosts = _stats.ActivePosts + _stats.DeletedPosts;
    }

    private async Task LoadCommentStatsAsync()
    {
        _stats.ActiveComments = await DbContext.Comments
            .AsNoTracking()
            .CountAsync();
        _stats.DeletedComments = await DbContext.Comments
            .AsNoTracking()
            .IgnoreQueryFilters()
            .CountAsync(c => c.IsDeleted);
        _stats.TotalComments = _stats.ActiveComments + _stats.DeletedComments;
    }

    private async Task LoadJobPostingStatsAsync()
    {
        _stats.ActiveJobPostings = await DbContext.JobPostings
            .AsNoTracking()
            .CountAsync();
        _stats.DeletedJobPostings = await DbContext.JobPostings
            .AsNoTracking()
            .IgnoreQueryFilters()
            .CountAsync(jp => jp.IsDeleted);
        _stats.TotalJobPostings = _stats.ActiveJobPostings + _stats.DeletedJobPostings;
    }

    private async Task LoadJobStatsAsync()
    {
        _stats.ActiveJobs = await DbContext.Jobs
            .AsNoTracking()
            .CountAsync();
        _stats.DeletedJobs = await DbContext.Jobs
            .AsNoTracking()
            .IgnoreQueryFilters()
            .CountAsync(j => j.IsDeleted);
        _stats.TotalJobs = _stats.ActiveJobs + _stats.DeletedJobs;
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

    private async Task LoadJobApplicationStatsAsync()
    {
        _stats.ActiveJobApplications = await DbContext.JobApplications
                .AsNoTracking()
                .CountAsync();
        _stats.DeletedJobApplications = await DbContext.JobApplications
                .AsNoTracking()
                .IgnoreQueryFilters()
                .CountAsync(ja => ja.IsDeleted);
        _stats.TotalJobApplications = _stats.ActiveJobApplications + _stats.DeletedJobApplications;
    }

    private class DashboardStats
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int DeletedUsers { get; set; }

        public int TotalPosts { get; set; }
        public int ActivePosts { get; set; }
        public int DeletedPosts { get; set; }

        public int TotalComments { get; set; }
        public int ActiveComments { get; set; }
        public int DeletedComments { get; set; }

        public int TotalJobPostings { get; set; }
        public int ActiveJobPostings { get; set; }
        public int DeletedJobPostings { get; set; }

        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public int DeletedJobs { get; set; }

        public int TotalProfiles { get; set; }
        public int ActiveProfiles { get; set; }
        public int DeletedProfiles { get; set; }

        public int TotalJobApplications { get; set; }
        public int ActiveJobApplications { get; set; }
        public int DeletedJobApplications { get; set; }
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
