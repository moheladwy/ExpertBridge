using ExpertBridge.Application.Models;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.PeriodicJobs;

public sealed class CleanUpNotificationsPeriodicWorker : PeriodicWorker<CleanUpNotificationsPeriodicWorker>
{

    private readonly IServiceProvider _services;
    private readonly ILogger<CleanUpNotificationsPeriodicWorker> _logger;
    private const int TimeIntervalForNotificationCleanupInDays = 30;

    public CleanUpNotificationsPeriodicWorker(
    IServiceProvider services,
            ILogger<CleanUpNotificationsPeriodicWorker> logger)
            : base(
                PeriodicJobsStartDelays.CleanUpNotificationsPeriodicWorkerStartDelay,
                nameof(CleanUpNotificationsPeriodicWorker),
                logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteInternalAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

            // Using ExecuteDeleteAsync for bulk deletion is more efficient than fetching and deleting individually:
            // It executes as a single SQL DELETE statement directly on the database server (avoiding multiple round trips)
            // and avoids loading potentially thousands of notifications into memory.
            var numNotificationsDeleted = await dbContext.Notifications
                .Where(n =>
                    n.CreatedAt < DateTime.UtcNow.AddDays(-TimeIntervalForNotificationCleanupInDays) &&
                    n.IsRead == true)
                .ExecuteDeleteAsync(cancellationToken: stoppingToken);

            // _logger.LogInformation("Deleted {numNotificationsDeleted} notifications older than 30 days and read",
            //     numNotificationsDeleted);
            Log.Information("Deleted {numNotificationsDeleted} notifications older than 30 days and read",
                numNotificationsDeleted);
        }
        catch (InvalidOperationException e)
        {
            // _logger.LogError(e, "An error occurred while executing the periodic worker {name}, {message}",
            //     nameof(CleanUpNotificationsPeriodicWorker), e.Message);
            Log.Error(e, "An error occurred while executing the periodic worker {name}, {message}",
                nameof(CleanUpNotificationsPeriodicWorker), e.Message);
        }
    }
}
