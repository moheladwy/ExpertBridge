using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
