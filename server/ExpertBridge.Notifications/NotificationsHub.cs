// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Notifications.Models;
using Microsoft.AspNetCore.SignalR;

namespace ExpertBridge.Notifications;

/// <summary>
///     Defines the client-side contract for receiving real-time notifications and messages via SignalR.
///     Implemented by JavaScript/TypeScript clients to handle incoming notifications and chat messages pushed from the
///     server.
/// </summary>
public interface INotificationClient
{
    /// <summary>
    ///     Client-side method invoked by the server to deliver a real-time notification.
    /// </summary>
    /// <param name="notification">The notification response containing message, action URL, and metadata.</param>
    /// <returns>A task representing the asynchronous client-side notification handling.</returns>
    Task ReceiveNotification(NotificationResponse notification);

    /// <summary>
    ///     Client-side method invoked by the server to deliver a real-time chat message.
    /// </summary>
    /// <param name="message">The message containing sender, content, and chat context information.</param>
    /// <returns>A task representing the asynchronous client-side message handling.</returns>
    Task ReceiveMessage(Message message);
}

/// <summary>
///     SignalR hub for bidirectional real-time communication between server and clients for notifications and chat
///     messages.
///     Provides WebSocket-based push delivery of notifications and chat messages to connected users.
/// </summary>
/// <remarks>
///     This hub implements the strongly-typed Hub&lt;INotificationClient&gt; pattern, ensuring type-safe client method
///     invocation.
///     **Connection Flow:**
///     1. Client establishes WebSocket connection to /api/notificationsHub endpoint
///     2. Hub assigns unique connection ID to each client
///     3. Server broadcasts notifications to connected clients via INotificationClient.ReceiveNotification
///     4. Server sends chat messages to specific clients via INotificationClient.ReceiveMessage
///     5. Client can retrieve connection ID for targeted messaging
///     **Use Cases:**
///     - Real-time notification delivery (comments, votes, job applications, offers)
///     - Live chat message delivery between users
///     - Connection management for presence tracking
///     **Security:**
///     Requires SignalRClients CORS policy configured in Extensions project.
///     Authentication typically handled via JWT tokens in connection handshake.
///     Hub lifecycle managed by ASP.NET Core SignalR infrastructure with automatic reconnection handling.
/// </remarks>
public sealed class NotificationsHub : Hub<INotificationClient>
{
    /// <summary>
    ///     Invoked when a client successfully establishes a SignalR connection to the hub.
    ///     Can be overridden to implement custom connection logic (e.g., presence tracking, user mapping).
    /// </summary>
    /// <returns>A task representing the asynchronous connection establishment.</returns>
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    /// <summary>
    ///     Invoked when a client disconnects from the SignalR hub either gracefully or due to connection failure.
    ///     Can be overridden to implement cleanup logic (e.g., presence updates, connection mapping removal).
    /// </summary>
    /// <param name="exception">The exception that caused the disconnection, or null for graceful disconnection.</param>
    /// <returns>A task representing the asynchronous disconnection handling.</returns>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    ///     Remotely callable method that returns the unique SignalR connection ID for the current client.
    ///     Used by clients to identify their connection for targeted message delivery or connection mapping.
    /// </summary>
    /// <returns>The unique connection identifier assigned by SignalR for this client connection.</returns>
    /// <remarks>
    ///     IMPORTANT: This MUST remain a method, not a property, to be callable from JavaScript/TypeScript clients.
    ///     SignalR remote procedure calls (RPCs) only work with methods, not properties.
    ///     Example client usage (JavaScript):
    ///     const connectionId = await connection.invoke("GetConnectionId");
    /// </remarks>
    public string GetConnectionId()
    {
        return Context.ConnectionId;
    }
}
