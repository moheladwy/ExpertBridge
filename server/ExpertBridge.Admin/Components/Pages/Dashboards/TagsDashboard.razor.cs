// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Admin.Components.Pages.Dashboards;

public partial class TagsDashboard : ComponentBase
{
    private readonly bool _showDataLabels;
    private readonly TagStats _stats;
    private readonly ExpertBridgeDbContext DbContext;
    private bool _loading;
    private List<ChartDataItem> _tagUsageChartData;

    public TagsDashboard(ExpertBridgeDbContext dbContext)
    {
        DbContext = dbContext;
        _loading = true;
        _showDataLabels = true;
        _stats = new TagStats();
        _tagUsageChartData = [];
    }

    protected override async Task OnInitializedAsync() => await RefreshData();

    private async Task RefreshData()
    {
        try
        {
            _loading = true;
            await LoadTagStatsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading tags data: {ex.Message}");
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task LoadTagStatsAsync()
    {
        _stats.TotalTags = await DbContext.Tags
            .AsNoTracking()
            .CountAsync();

        // Count unique tags used in User Interests
        _stats.UserInterestsCount = await DbContext.UserInterests
            .AsNoTracking()
            .Select(ui => ui.TagId)
            .Distinct()
            .CountAsync();

        // Count unique tags used in Post Tags
        _stats.PostTagsCount = await DbContext.PostTags
            .AsNoTracking()
            .Select(pt => pt.TagId)
            .Distinct()
            .CountAsync();

        // Count unique tags used in Job Posting Tags
        _stats.JobPostingTagsCount = await DbContext.JobPostingTags
            .AsNoTracking()
            .Select(jpt => jpt.TagId)
            .Distinct()
            .CountAsync();

        _stats.TotalTagUsage = _stats.UserInterestsCount + _stats.PostTagsCount + _stats.JobPostingTagsCount;

        // Populate chart data
        _tagUsageChartData =
        [
            new ChartDataItem { Category = "User Interests", Value = _stats.UserInterestsCount },
            new ChartDataItem { Category = "Post Tags", Value = _stats.PostTagsCount },
            new ChartDataItem { Category = "Job Posting Tags", Value = _stats.JobPostingTagsCount }
        ];
    }

    private class TagStats
    {
        public int TotalTags { get; set; }
        public int UserInterestsCount { get; set; }
        public int PostTagsCount { get; set; }
        public int JobPostingTagsCount { get; set; }
        public int TotalTagUsage { get; set; }
    }

    private class ChartDataItem
    {
        public string Category { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
