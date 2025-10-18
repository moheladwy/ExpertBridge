// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Notifications.BackgroundServices;
using ExpertBridge.Notifications.Models.IPC;
using Microsoft.Extensions.DependencyInjection;

namespace ExpertBridge.Notifications.Extensions;

/// <summary>
/// Provides dependency injection extension methods for configuring notification services in the application.
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Registers all notification-related services including SignalR, background workers, facades, and the notification channel pipeline.
    /// </summary>
    /// <param name="services">The service collection to add notification services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <remarks>
    /// This extension method configures a complete notification infrastructure with the following components:
    /// 
    /// **Infrastructure:**
    /// - SignalR: Enables real-time WebSocket communication for push notifications to connected clients
    /// 
    /// **Background Services:**
    /// - NotificationSendingPipelineHandlerWorker: Hosted service that processes queued notifications, persists to database, and broadcasts via SignalR
    /// 
    /// **Services:**
    /// - NotificationFacade: Scoped service providing high-level API for creating notifications from domain events
    /// 
    /// **IPC (Inter-Process Communication):**
    /// - Unbounded Channel&lt;SendNotificationsRequestMessage&gt;: Thread-safe queue connecting facade to background worker
    ///   - Registered as singleton for shared access across scopes
    ///   - Unbounded to prevent blocking during notification creation
    ///   - Provides backpressure handling and reliable message delivery
    /// 
    /// **Architecture Benefits:**
    /// - Decouples notification creation from delivery (non-blocking API operations)
    /// - Enables reliable delivery with retry and error handling
    /// - Scales independently (background worker can be optimized separately)
    /// - Maintains transaction isolation (notifications don't block main database operations)
    /// 
    /// Typical usage in Program.cs:
    /// <code>
    /// builder.Services.AddNotifications();
    /// app.MapHub&lt;NotificationsHub&gt;("/api/notificationsHub");
    /// </code>
    /// </remarks>
    public static IServiceCollection AddNotifications(this IServiceCollection services)
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
