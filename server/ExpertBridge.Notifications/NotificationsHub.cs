// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Notifications.Models;
using Microsoft.AspNetCore.SignalR;

namespace ExpertBridge.Notifications
{
    public interface INotificationClient
    {
        Task ReceiveNotification(NotificationResponse notification);
    }

    public class NotificationsHub : Hub<INotificationClient>
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        // This is a remotely callable method from the client side.
        // DO NOT TURN INTO A PROPERTY!
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
