// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ExpertBridge.Worker.PeriodicJobs.NotificationsCleaning;

internal sealed class NotificationsCleaningPeriodicWorker : IJob
{
    private const int TimeIntervalForNotificationCleanupInDays = 30;
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly ILogger<NotificationsCleaningPeriodicWorker> _logger;

    public NotificationsCleaningPeriodicWorker(
        ILogger<NotificationsCleaningPeriodicWorker> logger,
        ExpertBridgeDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

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
