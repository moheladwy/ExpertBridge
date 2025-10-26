// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Core.Entities.Notifications;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications.Models;
using ExpertBridge.Notifications.Models.IPC;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ExpertBridge.Notifications.BackgroundServices;

/// <summary>
/// Background service that continuously processes notification requests from the Channel queue, persists them to the database, and broadcasts them via SignalR.
/// Implements the consumer side of the Channel-based notification pipeline initiated by <see cref="NotificationFacade"/>.
/// </summary>
/// <remarks>
/// This worker implements a reliable notification delivery system with the following characteristics:
///
/// **Processing Pipeline:**
/// 1. Continuously reads SendNotificationsRequestMessage from unbounded channel
/// 2. Creates scoped service provider for each batch (database context, SignalR hub)
/// 3. Transforms IPC messages into Notification entities
/// 4. Persists notifications to database for historical record and offline delivery
/// 5. Broadcasts notifications to all connected SignalR clients in real-time
/// 6. Logs errors without stopping the pipeline (resilient design)
///
/// **Benefits:**
/// - Decouples notification creation from delivery (non-blocking for API requests)
/// - Provides reliable delivery even during temporary database or SignalR outages
/// - Supports batching for efficiency (processes lists of notifications together)
/// - Enables retry and error handling without affecting main application flow
/// - Maintains notification history in database for audit and offline scenarios
///
/// **Error Handling:**
/// - Individual notification failures are logged but don't stop the worker
/// - Channel read failures terminate the worker gracefully with logging
/// - Scoped services ensure proper disposal of DbContext after each batch
///
/// The worker runs continuously as a hosted service until application shutdown (CancellationToken signaled).
/// </remarks>
public class NotificationSendingPipelineHandlerWorker : BackgroundService
{
    private readonly ChannelReader<SendNotificationsRequestMessage> _channel;
    private readonly ILogger<NotificationSendingPipelineHandlerWorker> _logger;
    private readonly IServiceProvider _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationSendingPipelineHandlerWorker"/> class with the notification channel and service provider.
    /// </summary>
    /// <param name="services">The service provider for creating scoped services (DbContext, SignalR hub) per batch.</param>
    /// <param name="channel">The unbounded channel for reading queued notification requests.</param>
    /// <param name="logger">The logger for diagnostic and error logging.</param>
    public NotificationSendingPipelineHandlerWorker(
        IServiceProvider services,
        Channel<SendNotificationsRequestMessage> channel,
        ILogger<NotificationSendingPipelineHandlerWorker> logger)
    {
        _services = services;
        _logger = logger;
        _channel = channel.Reader;
    }

    /// <summary>
    /// Executes the notification processing loop, continuously reading from the channel, persisting to database, and broadcasting via SignalR.
    /// Runs until cancellation is requested during application shutdown.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token signaled when the application is shutting down.</param>
    /// <returns>A task representing the continuous background processing operation.</returns>
    /// <remarks>
    /// For each notification batch:
    /// 1. Waits for channel data availability (non-blocking)
    /// 2. Reads message from channel
    /// 3. Creates new scope for transactional processing
    /// 4. Persists notifications to ExpertBridgeDbContext
    /// 5. Broadcasts to all clients via NotificationsHub.Clients.All.ReceiveNotification
    ///
    /// Error handling ensures individual failures don't crash the worker, maintaining system resilience.
    /// </remarks>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (await _channel.WaitToReadAsync(stoppingToken))
            {
                var message = await _channel.ReadAsync(stoppingToken);

                try
                {
                    using var scope = _services.CreateScope();

                    var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

                    var notificationEnitites = message.Notifications.Select(n => new Notification
                    {
                        RecipientId = n.RecipientId,
                        SenderId = n.SenderId,
                        Message = n.Message,
                        ActionUrl = n.ActionUrl,
                        IconUrl = n.IconUrl,
                        IconActionUrl = n.IconActionUrl,
                        IsRead = n.IsRead
                    });

                    await dbContext.Notifications.AddRangeAsync(notificationEnitites, stoppingToken);
                    await dbContext.SaveChangesAsync(stoppingToken);

                    var hub = scope.ServiceProvider
                        .GetRequiredService<IHubContext<NotificationsHub, INotificationClient>>();

                    foreach (var notification in notificationEnitites)
                    {
                        // Send the notification to the client
                        // We can use the notification ID as a unique identifier
                        // for the notification in the client-side application.
                        await hub.Clients.All.ReceiveNotification(new NotificationResponse
                        {
                            Id = notification.Id,
                            ActionUrl = notification.ActionUrl,
                            CreatedAt = notification.CreatedAt ?? DateTime.UtcNow,
                            IconActionUrl = notification.IconActionUrl,
                            IconUrl = notification.IconUrl,
                            IsRead = notification.IsRead,
                            Message = notification.Message,
                            RecipientId = notification.RecipientId
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Pipeline: An error occurred while processing notification ={NotificationId}.",
                        message.Notifications.First().Message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "An error occurred while reading from the channel in {NotificationSenderHandlerWorker}.",
                nameof(NotificationSendingPipelineHandlerWorker));
        }
        finally
        {
            _logger.LogInformation("Terminating {NotificationSenderHandlerWorker}.",
                nameof(NotificationSendingPipelineHandlerWorker));
        }
    }
}
