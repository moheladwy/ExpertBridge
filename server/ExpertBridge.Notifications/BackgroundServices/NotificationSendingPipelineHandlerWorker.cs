// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

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

public class NotificationSendingPipelineHandlerWorker : BackgroundService
{
    private readonly ChannelReader<SendNotificationsRequestMessage> _channel;
    private readonly ILogger<NotificationSendingPipelineHandlerWorker> _logger;
    private readonly IServiceProvider _services;

    public NotificationSendingPipelineHandlerWorker(
        IServiceProvider services,
        Channel<SendNotificationsRequestMessage> channel,
        ILogger<NotificationSendingPipelineHandlerWorker> logger)
    {
        _services = services;
        _logger = logger;
        _channel = channel.Reader;
    }

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
