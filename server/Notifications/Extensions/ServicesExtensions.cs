using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Notifications.Models.IPC;


namespace Notifications.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddExpertBridgeNotifications(this IServiceCollection services)
        {
            // Infrastructrue
            services.AddSignalR();

            // Background services
            services.AddHostedService<NotificationSenderHandlerWorker>();

            // IPC
            services.AddSingleton(_ => Channel.CreateUnbounded<SendNotificationMessage>());

            return services;
        }
    }
}
