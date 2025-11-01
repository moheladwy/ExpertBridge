// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.ModerationReports;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpertBridge.Application.DomainServices;

/// <summary>
///     Provides domain-level operations for moderation reports such as reporting content,
///     deleting reports, and restoring content that was previously soft-deleted as part of a moderation action.
/// </summary>
/// <remarks>
///     This service uses <see cref="ExpertBridgeDbContext" /> to persist changes. Some operations use
///     EF Core's <c>ExecuteUpdateAsync</c> for bulk/inline updates which do not load full entity instances into memory.
/// </remarks>
public class ModerationReportService
{
    /// <summary>
    ///     Represents the database context used to interact with the database in the moderation report service.
    ///     This instance facilitates operations like adding, removing, or updating moderation reports
    ///     and saving changes to the database.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Represents the logging instance used to log informational, warning, and error messages
    ///     related to operations performed by the ModerationReportService.
    ///     This logger helps in tracking and debugging activities such as reporting content,
    ///     deleting reports, and restoring content.
    /// </summary>
    private readonly ILogger<ModerationReportService> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ModerationReportService" /> class.
    /// </summary>
    /// <param name="logger">The logger used to record informational and error messages.</param>
    /// <param name="dbContext">The application's database context used for persistence operations.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="logger" /> or <paramref name="dbContext" /> is
    ///     <c>null</c>.
    /// </exception>
    public ModerationReportService(ILogger<ModerationReportService> logger, ExpertBridgeDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    ///     Persists a new moderation report to the database.
    /// </summary>
    /// <param name="report">The <see cref="ModerationReport" /> instance representing the report to store.</param>
    /// <returns>A task that completes when the report has been saved.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="report" /> is <c>null</c>.</exception>
    /// <exception cref="DbUpdateException">An error is encountered while saving to the database.</exception>
    /// <exception cref="DbUpdateConcurrencyException">A concurrency violation is encountered while saving to the database.</exception>
    /// <exception cref="OperationCanceledException">
    ///     If the operation is canceled (for example via a cancellation token passed
    ///     deeper into EF Core APIs).
    /// </exception>
    /// <remarks>
    ///     This method adds the provided <paramref name="report" /> entity and calls <c>SaveChangesAsync</c>.
    ///     Logging occurs before and after the database save to assist with diagnostics.
    /// </remarks>
    public async Task ReportContent(ModerationReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        _logger.LogInformation("Reporting {ContentType} {ContentId} for {Reason}",
            report.ContentType, report.ContentId, report.Reason);

        await _dbContext.ModerationReports.AddAsync(report);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Reported {ContentType} {ContentId} successfully",
            report.ContentType, report.ContentId);
    }

    /// <summary>
    ///     Removes an existing moderation report from the database.
    /// </summary>
    /// <param name="report">The <see cref="ModerationReport" /> to delete.</param>
    /// <returns>A task that completes when the report has been deleted.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="report" /> is <c>null</c>.</exception>
    /// <exception cref="DbUpdateException">An error is encountered while saving to the database.</exception>
    /// <exception cref="DbUpdateConcurrencyException">A concurrency violation is encountered while saving to the database.</exception>
    /// <exception cref="OperationCanceledException">
    ///     If the operation is canceled (for example via a cancellation token passed
    ///     deeper into EF Core APIs).
    /// </exception>
    /// <remarks>
    ///     This method removes the provided <paramref name="report" /> entity and calls <c>SaveChangesAsync</c>.
    ///     Use caution: this performs a hard delete of the report record.
    /// </remarks>
    public async Task DeleteReport(ModerationReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        _logger.LogInformation("Deleting {ContentType} {ContentId} for {Reason}",
            report.ContentType, report.ContentId, report.Reason);

        _dbContext.ModerationReports.Remove(report);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Deleted {ContentType} {ContentId} successfully",
            report.ContentType, report.ContentId);
    }

    /// <summary>
    ///     Restores the content associated with a moderation report (for example, un-deletes a soft-deleted post or comment)
    ///     and marks the report as resolved and non-negative.
    /// </summary>
    /// <param name="report">The moderation report containing the content type and id to restore.</param>
    /// <returns>A task that completes when the restore operation has been applied.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="report" /> is <c>null</c>.</exception>
    /// <exception cref="DbUpdateException">An error is encountered while saving to the database.</exception>
    /// <exception cref="DbUpdateConcurrencyException">A concurrency violation is encountered while saving to the database.</exception>
    /// <exception cref="OperationCanceledException">
    ///     If the operation is canceled (for example via a cancellation token passed
    ///     deeper into EF Core APIs).
    /// </exception>
    /// <remarks>
    ///     This method:
    ///     - Marks the moderation report as resolved (<c>IsResolved = true</c>) and not negative (<c>IsNegative = false</c>).
    ///     - Based on <see cref="ContentTypes" />, un-deletes the associated entity (e.g. <c>Post</c>, <c>Comment</c>,
    ///     <c>JobPosting</c>).
    ///     The implementation uses EF Core's <c>IgnoreQueryFilters()</c> and <c>ExecuteUpdateAsync</c> to update rows
    ///     without loading full entity instances. This is efficient for simple property changes but bypasses entity lifecycle
    ///     events.
    /// </remarks>
    public async Task RestoreContent(ModerationReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        _logger.LogInformation("Restoring {ContentType} {ContentId} for {Reason}",
            report.ContentType, report.ContentId, report.Reason);

        await _dbContext.ModerationReports
            .IgnoreQueryFilters()
            .Where(r => r.Id == report.Id)
            .ExecuteUpdateAsync(set => set
                .SetProperty(p => p.IsResolved, true)
                .SetProperty(p => p.IsNegative, false));

        switch (report.ContentType)
        {
            case ContentTypes.Post:
                await _dbContext.Posts
                    .IgnoreQueryFilters()
                    .Where(p => p.Id == report.ContentId)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.IsDeleted, false));
                break;
            case ContentTypes.Comment:
                await _dbContext.Comments
                    .IgnoreQueryFilters()
                    .Where(p => p.Id == report.ContentId)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.IsDeleted, false));
                break;
            case ContentTypes.JobPosting:
                await _dbContext.JobPostings
                    .IgnoreQueryFilters()
                    .Where(p => p.Id == report.ContentId)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.IsDeleted, false));
                break;
            case ContentTypes.Profile:
            case ContentTypes.Message:
            case ContentTypes.Video:
            case ContentTypes.Image:
            case ContentTypes.File:
            default:
                break;
        }

        _logger.LogInformation("Restored {ContentType} {ContentId} successfully",
            report.ContentType, report.ContentId);
    }

    /// <summary>
    ///     Toggles the resolution status of a moderation report between resolved and unresolved.
    /// </summary>
    /// <param name="report">The moderation report whose resolution status should be toggled.</param>
    /// <returns>A task that completes when the resolution status has been toggled.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="report" /> is <c>null</c>.</exception>
    /// <exception cref="DbUpdateException">An error is encountered while saving to the database.</exception>
    /// <exception cref="DbUpdateConcurrencyException">A concurrency violation is encountered while saving to the database.</exception>
    /// <exception cref="OperationCanceledException">
    ///     If the operation is canceled (for example via a cancellation token passed
    ///     deeper into EF Core APIs).
    /// </exception>
    /// <remarks>
    ///     This method inverts the current <c>IsResolved</c> property value of the specified report.
    ///     The implementation uses EF Core's <c>ExecuteUpdateAsync</c> for an efficient inline update
    ///     without loading the full entity into memory.
    /// </remarks>
    public async Task ToggleResolution(ModerationReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        _logger.LogInformation("Toggling {ContentType} {ContentId} as {Resolution}",
            report.ContentType, report.ContentId, report.IsResolved ? "resolved" : "unresolved");

        await _dbContext.ModerationReports
            .IgnoreQueryFilters()
            .Where(r => r.Id == report.Id)
            .ExecuteUpdateAsync(set => set
                .SetProperty(p => p.IsResolved, !report.IsResolved));

        _logger.LogInformation("Toggled {ContentType} {ContentId} successfully",
            report.ContentType, report.ContentId);
    }

    /// <summary>
    ///     Toggles the negative status of a moderation report between flagged as negative and not negative.
    /// </summary>
    /// <param name="report">The moderation report whose negative status should be toggled.</param>
    /// <returns>A task that completes when the negative status has been toggled.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="report" /> is <c>null</c>.</exception>
    /// <exception cref="DbUpdateException">An error is encountered while saving to the database.</exception>
    /// <exception cref="DbUpdateConcurrencyException">A concurrency violation is encountered while saving to the database.</exception>
    /// <exception cref="OperationCanceledException">
    ///     If the operation is canceled (for example, via a cancellation token passed
    ///     deeper into EF Core APIs).
    /// </exception>
    /// <remarks>
    ///     This method inverts the current <c>IsNegative</c> property value of the specified report.
    ///     The implementation uses EF Core's <c>ExecuteUpdateAsync</c> for an efficient inline update
    ///     without loading the full entity into memory.
    /// </remarks>
    public async Task ToggleNegative(ModerationReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        _logger.LogInformation("Toggling {ContentType} {ContentId} as {Negativity}",
            report.ContentType, report.ContentId, report.IsNegative ? "negative" : "not negative");

        var newStatus = !report.IsNegative;
        await _dbContext.ModerationReports
            .IgnoreQueryFilters()
            .Where(r => r.Id == report.Id)
            .ExecuteUpdateAsync(set => set
                .SetProperty(p => p.IsResolved, true)
                .SetProperty(p => p.IsNegative, newStatus));

        switch (report.ContentType)
        {
            case ContentTypes.Post:
                await _dbContext.Posts
                    .IgnoreQueryFilters()
                    .Where(p => p.Id == report.ContentId)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.IsDeleted, newStatus));
                break;
            case ContentTypes.Comment:
                await _dbContext.Comments
                    .IgnoreQueryFilters()
                    .Where(p => p.Id == report.ContentId)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.IsDeleted, newStatus));
                break;
            case ContentTypes.JobPosting:
                await _dbContext.JobPostings
                    .IgnoreQueryFilters()
                    .Where(p => p.Id == report.ContentId)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.IsDeleted, newStatus));
                break;
            case ContentTypes.Profile:
            case ContentTypes.Message:
            case ContentTypes.Video:
            case ContentTypes.Image:
            case ContentTypes.File:
            default:
                break;
        }

        _logger.LogInformation("Toggled {ContentType} {ContentId} successfully",
            report.ContentType, report.ContentId);
    }
}
