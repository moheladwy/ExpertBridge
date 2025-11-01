// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.ModerationReports;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Radzen;
using Radzen.Blazor;

namespace ExpertBridge.Admin.Components.Pages.ModerationReports;

public partial class ModerationReports : ComponentBase
{
    private readonly List<string> negativeFilters = ["Flagged Only", "Clean Only"];
    private readonly List<string> resolutionStatuses = ["Resolved", "Unresolved"];
    private List<ModerationReport> allReports = [];

    // Filter options
    private List<string> contentTypes = [];
    private RadzenDataGrid<ModerationReport>? grid;
    private bool isLoading = true;
    private List<string> reportedByOptions = [];
    private List<ModerationReport> reports = [];
    private int resolvedCount;
    private string searchText = string.Empty;

    // Selected filters
    private string? selectedContentType;
    private string? selectedNegativeFilter;
    private string? selectedReportedBy;
    private string? selectedResolutionStatus;

    // Statistics
    private int totalCount;
    private int unresolvedCount;

    [Inject] private ExpertBridgeDbContext DbContext { get; set; } = default!;

    [Inject] private HybridCache Cache { get; set; } = default!;

    [Inject] private DialogService DialogService { get; set; } = default!;

    [Inject] private NotificationService NotificationService { get; set; } = default!;

    [Inject] private ILogger<ModerationReports> Logger { get; set; } = default!;

    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private bool HasActiveFilters
    {
        get
        {
            return !string.IsNullOrWhiteSpace(selectedContentType) ||
                   !string.IsNullOrWhiteSpace(selectedResolutionStatus) ||
                   !string.IsNullOrWhiteSpace(selectedNegativeFilter) ||
                   !string.IsNullOrWhiteSpace(selectedReportedBy) ||
                   !string.IsNullOrWhiteSpace(searchText);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            // Populate content type filter options from enum
            contentTypes = Enum.GetNames<ContentTypes>().ToList();
            // Populate reported by filter options from enum
            reportedByOptions = Enum.GetNames<ReportedBy>().ToList();
            await LoadReportsAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing ModerationReports page");
            ShowNotification(NotificationSeverity.Error, "Error", "Failed to load moderation reports");
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task LoadReportsAsync()
    {
        isLoading = true;

        try
        {
            allReports = await Cache.GetOrCreateAsync(
                "admin:moderation-reports:all",
                async cancel => await DbContext.ModerationReports
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync(cancel),
                cancellationToken: default
            );

            UpdateStatistics();
            ApplyFilters();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading moderation reports");
            ShowNotification(NotificationSeverity.Error, "Error", "Failed to load moderation reports");
        }
        finally
        {
            isLoading = false;
        }
    }

    private void UpdateStatistics()
    {
        totalCount = allReports.Count;
        resolvedCount = allReports.Count(r => r.IsResolved);
        unresolvedCount = allReports.Count(r => !r.IsResolved);
    }

    private void ApplyFilters()
    {
        reports = allReports;

        // Filter by content type
        if (!string.IsNullOrWhiteSpace(selectedContentType))
        {
            if (Enum.TryParse<ContentTypes>(selectedContentType, out var contentType))
            {
                reports = reports.Where(r => r.ContentType == contentType).ToList();
            }
        }

        // Filter by resolution status
        if (!string.IsNullOrWhiteSpace(selectedResolutionStatus))
        {
            var isResolved = selectedResolutionStatus == "Resolved";
            reports = reports.Where(r => r.IsResolved == isResolved).ToList();
        }

        // Filter by negative content
        if (!string.IsNullOrWhiteSpace(selectedNegativeFilter))
        {
            var showFlagged = selectedNegativeFilter == "Flagged Only";
            reports = reports.Where(r => r.IsNegative == showFlagged).ToList();
        }

        // Filter by reported by
        if (!string.IsNullOrWhiteSpace(selectedReportedBy))
        {
            if (Enum.TryParse<ReportedBy>(selectedReportedBy, out var reportedBy))
            {
                reports = reports.Where(r => r.ReportedBy == reportedBy).ToList();
            }
        }

        // Filter by search text
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            reports = reports.Where(r =>
                (r.Id?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (r.ContentId?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (r.AuthorId?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (r.Reason?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();
        }
    }

    private void OnFilterChanged()
    {
        ApplyFilters();
    }

    private void ClearFilters()
    {
        selectedContentType = null;
        selectedResolutionStatus = null;
        selectedNegativeFilter = null;
        selectedReportedBy = null;
        searchText = string.Empty;
        ApplyFilters();
    }

    private async Task RefreshData()
    {
        // Invalidate cache and reload
        await Cache.RemoveAsync("admin:moderation-reports:all");
        await LoadReportsAsync();
        ShowNotification(NotificationSeverity.Success, "Refreshed", "Moderation reports data has been refreshed");
    }

    private void OpenDetailsDialog(ModerationReport report)
    {
        NavigationManager.NavigateTo($"/moderation-reports/{report.Id}");
    }

    private static void OnRowClick(DataGridRowMouseEventArgs<ModerationReport> args)
    {
        // Optional: Open details dialog when clicking a row
        // OpenDetailsDialog(args.Data);
    }

    private static BadgeStyle GetContentTypeBadgeStyle(ContentTypes contentType)
    {
        return contentType switch
        {
            ContentTypes.Post => BadgeStyle.Success,
            ContentTypes.Comment => BadgeStyle.Info,
            ContentTypes.JobPosting => BadgeStyle.Warning,
            ContentTypes.Profile => BadgeStyle.Secondary,
            ContentTypes.Message => BadgeStyle.Primary,
            ContentTypes.Video => BadgeStyle.Danger,
            ContentTypes.Image => BadgeStyle.Light,
            ContentTypes.File => BadgeStyle.Dark,
            _ => BadgeStyle.Light
        };
    }

    private static BadgeStyle GetReportedByBadgeStyle(ReportedBy reportedBy)
    {
        return reportedBy switch
        {
            ReportedBy.Ai => BadgeStyle.Info,
            ReportedBy.User => BadgeStyle.Warning,
            ReportedBy.Admin => BadgeStyle.Danger,
            _ => BadgeStyle.Light
        };
    }

    private static string GetScoreBarStyle(double score)
    {
        // Color code based on severity: green (low), yellow (medium), orange (high), red (critical)
        var color = score switch
        {
            < 0.3 => "var(--rz-success)",
            < 0.5 => "var(--rz-warning)",
            < 0.7 => "var(--rz-danger)",
            _ => "var(--rz-danger)"
        };

        return $"height: 8px; width: 60px; --rz-series-0: {color};";
    }

    private void ShowNotification(NotificationSeverity severity, string summary, string detail)
    {
        NotificationService.Notify(new NotificationMessage
        {
            Severity = severity, Summary = summary, Detail = detail, Duration = 4000
        });
    }
}
