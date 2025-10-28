// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ExpertBridge.Worker.PeriodicJobs.NotificationsCleaning;

/// <summary>
///     Periodic worker job responsible for cleaning up old read notifications from the database.
/// </summary>
/// <remarks>
///     This job removes notifications that are older than 30 days and have been marked as read,
///     helping to maintain database performance and storage efficiency.
/// </remarks>
internal sealed class NotificationsCleaningPeriodicWorker : IJob
{
    /// <summary>
    ///     The time interval in days after which read notifications are eligible for cleanup.
    /// </summary>
    private const int TimeIntervalForNotificationCleanupInDays = 30;

    /// <summary>
    ///     Database context for accessing and deleting notifications.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Logger instance for logging job execution results.
    /// </summary>
    private readonly ILogger<NotificationsCleaningPeriodicWorker> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotificationsCleaningPeriodicWorker" /> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="dbContext">The database context.</param>
    public NotificationsCleaningPeriodicWorker(
        ILogger<NotificationsCleaningPeriodicWorker> logger,
        ExpertBridgeDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    ///     Executes the notification cleanup job.
    /// </summary>
    /// <param name="context">The job execution context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     Uses ExecuteDeleteAsync for efficient bulk deletion, executing as a single SQL DELETE statement
    ///     directly on the database server without loading entities into memory.
    /// </remarks>
    public async Task Execute(IJobExecutionContext context)
    {
        // Using ExecuteDeleteAsync for bulk deletion is more efficient than fetching and deleting individually:
        // It executes as a single SQL DELETE statement directly on the database server (avoiding multiple round trips)
        // and avoids loading potentially thousands of notifications into memory.
        var numNotificationsDeleted = await _dbContext.Notifications
            .Where(n =>
                n.CreatedAt < DateTime.UtcNow.AddDays(-TimeIntervalForNotificationCleanupInDays) &&
                n.IsRead == true)
            .ExecuteDeleteAsync(context.CancellationToken);

        _logger.LogInformation("Deleted {numNotificationsDeleted} notifications older than 30 days and read",
            numNotificationsDeleted);
    }
}
