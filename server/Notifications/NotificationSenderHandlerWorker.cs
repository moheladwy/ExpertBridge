using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notifications.Models;
using Notifications.Models.IPC;

namespace Notifications
{
    public class NotificationSenderHandlerWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<NotificationSenderHandlerWorker> _logger;
        private readonly ChannelReader<SendNotificationMessage> _channel;

        public NotificationSenderHandlerWorker(
            IServiceProvider services,
            Channel<SendNotificationMessage> channel,
            ILogger<NotificationSenderHandlerWorker> logger)
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
                    var notification = await _channel.ReadAsync(stoppingToken);

                    try
                    {
                        using var scope = _services.CreateScope();
                        var hub = scope.ServiceProvider
                            .GetRequiredService<IHubContext<NotificationsHub, INotificationClient>>();

                        await hub.Clients.All.ReceiveNotification(new Notification
                        {
                            Id = notification.NotificationId,
                            ActionUrl = notification.ActionUrl,
                            CreatedAt = notification.CreatedAt,
                            IconActionUrl = notification.IconActionUrl,
                            IconUrl = notification.IconUrl,
                            IsRead = notification.IsRead,
                            Message = notification.Message,
                            RecipientId = notification.RecipientId,
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Pipeline: An error occurred while processing notification with id={NotificationId}.",
                            notification.NotificationId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An error occurred while reading from the channel in {NotificationSenderHandlerWorker}.",
                    nameof(NotificationSenderHandlerWorker));
            }
            finally
            {
                _logger.LogInformation("Terminating {NotificationSenderHandlerWorker}.",
                    nameof(NotificationSenderHandlerWorker));
            }
        }
    }
}
