// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Notifications;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using ExpertBridge.Notifications.Models;
using ExpertBridge.Notifications.Models.IPC;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace ExpertBridge.Worker.Consumers;

/// <summary>
/// Consumes <see cref="SendNotificationsRequestMessage"/> messages from the message bus
/// and forwards the notifications to connected clients via SignalR while persisting them
/// to the database.
/// </summary>
public sealed class NotificationSendingPipelineConsumer : IConsumer<SendNotificationsRequestMessage>
{
    private readonly ILogger<NotificationSendingPipelineConsumer> _logger;
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly IHubContext<NotificationsHub, INotificationClient> _hubContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationSendingPipelineConsumer"/> class.
    /// </summary>
    /// <param name="logger">The logger instance used for diagnostics and tracking notification processing.</param>
    /// <param name="dbContext">The database context for persisting notifications to the database.</param>
    /// <param name="hubContext">The SignalR hub context used to broadcast notifications to connected clients.</param>
    public NotificationSendingPipelineConsumer(
        ILogger<NotificationSendingPipelineConsumer> logger,
        ExpertBridgeDbContext dbContext, IHubContext<NotificationsHub,
            INotificationClient> hubContext)
    {
        _logger = logger;
        _dbContext = dbContext;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Handles the consumption of a <see cref="SendNotificationsRequestMessage"/> message.
    /// The method persists the notifications to the database and broadcasts them to all
    /// connected SignalR clients.
    /// </summary>
    /// <param name="context">The consume context containing the message and cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// This method performs the following steps:
    /// <list type="number">
    /// <item><description>Maps the notification data from the message to <see cref="Notification"/> entities.</description></item>
    /// <item><description>Persists all notifications to the database.</description></item>
    /// <item><description>Broadcasts each notification to all connected SignalR clients.</description></item>
    /// </list>
    /// Any exceptions that occur during processing are caught and logged, but do not propagate.
    /// </remarks>
    public async Task Consume(ConsumeContext<SendNotificationsRequestMessage> context)
    {
        try
        {
            _logger.LogInformation("Sending notifications to clients.");
            var message = context.Message;

            var notifications = message.Notifications.Select(n => new Notification
            {
                RecipientId = n.RecipientId,
                SenderId = n.SenderId,
                Message = n.Message,
                ActionUrl = n.ActionUrl,
                IconUrl = n.IconUrl,
                IconActionUrl = n.IconActionUrl,
                IsRead = n.IsRead
            }).ToList();

            await _dbContext.Notifications.AddRangeAsync(notifications, context.CancellationToken);
            await _dbContext.SaveChangesAsync(context.CancellationToken);

            foreach (var notification in notifications)
            {
                // Send the notification to the client
                // We can use the notification ID as a unique identifier
                // for the notification in the client-side application.
                await _hubContext.Clients.All.ReceiveNotification(new NotificationResponse
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

            _logger.LogInformation("Notifications sent to clients.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending notifications to clients.");
        }
    }
}
