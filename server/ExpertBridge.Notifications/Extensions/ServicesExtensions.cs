// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Notifications.BackgroundServices;
using ExpertBridge.Notifications.Models.IPC;
using Microsoft.Extensions.DependencyInjection;


namespace ExpertBridge.Notifications.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddExpertBridgeNotifications(this IServiceCollection services)
        {
            // Infrastructrue
            services.AddSignalR();

            // Background services
            services.AddHostedService<NotificationSendingPipelineHandlerWorker>();

            // Services
            services.AddScoped<NotificationFacade>();

            // IPC
            services.AddSingleton(_ => Channel.CreateUnbounded<SendNotificationsRequestMessage>());

            return services;
        }
    }
}
