# ExpertBridge.Notifications - GitHub Copilot Instructions

## Project Purpose

SignalR-based real-time notification system providing instant push notifications and messaging to connected clients. Handles notification persistence and distribution via Channel-based background processing.

## Architecture Role

**Real-time Communication Layer** - Manages WebSocket connections through SignalR hubs, processes notification requests through channels, and distributes notifications to connected clients in real-time.

## Key Responsibilities

-   SignalR hub for WebSocket connections
-   Real-time notification distribution
-   Message broadcasting
-   Channel-based notification processing
-   Notification persistence coordination
-   Connection lifecycle management

## Project Dependencies

```xml
Dependencies:
- ExpertBridge.Core (entities, interfaces)
- ExpertBridge.Data (persistence)

External:
- Microsoft.AspNetCore.SignalR
- System.Threading.Channels
```

## SignalR Hub Pattern

### Hub with Strongly-Typed Client Interface

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Notifications.Models;
using Microsoft.AspNetCore.SignalR;

namespace ExpertBridge.Notifications;

/// <summary>
///     Strongly-typed client interface defining methods that can be called on connected clients.
/// </summary>
public interface INotificationClient
{
    Task ReceiveNotification(NotificationResponse notification);
    Task ReceiveMessage(Message message);
}

/// <summary>
///     SignalR hub for real-time notifications and messaging.
/// </summary>
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

    /// <summary>
    ///     This is a remotely callable method from the client side.
    ///     DO NOT TURN INTO A PROPERTY!
    /// </summary>
    public string GetConnectionId()
    {
        return Context.ConnectionId;
    }
}
```

### Hub Registration in Program.cs

```csharp
using ExpertBridge.Notifications;
using ExpertBridge.Extensions.CORS;

var builder = WebApplication.CreateBuilder(args);

// ... other services

builder.Services.AddSignalR();

var app = builder.Build();

// SignalR must use CORS policy with AllowCredentials
app.UseCors(CorsPolicyNames.SignalRClients);

app.MapHub<NotificationsHub>("/api/notificationsHub");

app.Run();
```

## Notification Facade Pattern

### Facade for Notification Operations

```csharp
namespace ExpertBridge.Notifications;

/// <summary>
///     This Facade is responsible for handling notifications.
///     Sending them to the notification channel for processing by the Hub
///     and sending them to the database for persistence.
/// </summary>
public class NotificationFacade
{
    private readonly Channel<SendNotificationsRequestMessage> _channel;

    public NotificationFacade(
        Channel<SendNotificationsRequestMessage> channel)
    {
        _channel = channel;
    }

    public async Task NotifyNewCommentAsync(Comment comment)
    {
        if (comment.ParentComment != null)
        {
            await NotifyNewReplyAsync(comment);
        }
        else
        {
            await NotifyInternalAsync(new Notification
            {
                RecipientId = comment.Post?.AuthorId ?? comment.JobPosting?.AuthorId,
                Message = $"{comment.Author.FirstName} commented on your post: {comment.Content}",
                ActionUrl = $"/{(comment.PostId != null ? "posts" : "jobPostings")}/{comment.PostId ?? comment.JobPostingId}/#comment-{comment.Id}",
                IconUrl = comment.Author.ProfilePictureUrl,
                IconActionUrl = $"/profile/{comment.AuthorId}",
                SenderId = comment.AuthorId,
            });
        }
    }

    public async Task NotifyNewReplyAsync(Comment comment)
    {
        var notifications = new List<Notification>
        {
            // Notify post owner
            new Notification
            {
                RecipientId = comment.Post?.AuthorId ?? comment.JobPosting?.AuthorId,
                Message = $"{comment.Author.FirstName} replied to a comment on your post: {comment.Content}",
                ActionUrl = $"/{(comment.PostId != null ? "posts" : "jobPostings")}/{comment.PostId ?? comment.JobPostingId}/#comment-{comment.Id}",
                IconUrl = comment.Author.ProfilePictureUrl,
                IconActionUrl = $"/profile/{comment.AuthorId}",
                SenderId = comment.AuthorId,
            }
        };

        if (comment.ParentComment != null)
        {
            // Notify comment owner (new reply)
            notifications.Add(new Notification
            {
                RecipientId = comment.ParentComment.AuthorId,
                Message = $"{comment.Author.FirstName} replied to your comment: {comment.Content}",
                ActionUrl = $"/{(comment.PostId != null ? "posts" : "jobPostings")}/{comment.PostId ?? comment.JobPostingId}/#comment-{comment.Id}",
                IconUrl = comment.Author.ProfilePictureUrl,
                IconActionUrl = $"/profile/{comment.AuthorId}",
                SenderId = comment.AuthorId,
            });
        }

        await NotifyInternalAsync(notifications);
    }

    public async Task NotifyCommentVotedAsync(CommentVote vote)
    {
        ArgumentNullException.ThrowIfNull(vote);

        await NotifyInternalAsync(new Notification
        {
            RecipientId = vote.Comment.AuthorId,
            Message = $"Your comment \"{vote.Comment.Content}\" received a new vote",
            ActionUrl = $"{(vote.Comment.PostId != null ? "posts" : "jobPostings")}/{vote.Comment.PostId ?? vote.Comment.JobPostingId}/#comment-{vote.Comment.Id}",
            IconUrl = vote.Comment.Author.ProfilePictureUrl,
            IconActionUrl = $"/profile",
            SenderId = vote.ProfileId,
        });
    }

    public async Task NotifyPostVotedAsync(PostVote vote)
    {
        ArgumentNullException.ThrowIfNull(vote);

        await NotifyInternalAsync(new Notification
        {
            RecipientId = vote.Post.AuthorId,
            Message = $"Your post \"{vote.Post.Content}\" received a new vote",
            ActionUrl = $"/posts/{vote.PostId}",
            IconUrl = vote.Profile.ProfilePictureUrl,
            IconActionUrl = $"/profile/{vote.ProfileId}",
            SenderId = vote.ProfileId,
        });
    }

    private async Task NotifyInternalAsync(Notification notification)
    {
        await _channel.Writer.WriteAsync(new SendNotificationsRequestMessage
        {
            Notifications = [notification]
        });
    }

    private async Task NotifyInternalAsync(List<Notification> notifications)
    {
        await _channel.Writer.WriteAsync(new SendNotificationsRequestMessage
        {
            Notifications = notifications
        });
    }
}
```

## Channel-Based Background Processing

### Background Worker for Notification Processing

```csharp
namespace ExpertBridge.Notifications.BackgroundServices;

public class NotificationSendingPipelineHandlerWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<NotificationSendingPipelineHandlerWorker> _logger;
    private readonly ChannelReader<SendNotificationsRequestMessage> _channel;

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

                    var notificationEntities = message.Notifications.Select(n => new Notification
                    {
                        RecipientId = n.RecipientId,
                        SenderId = n.SenderId,
                        Message = n.Message,
                        ActionUrl = n.ActionUrl,
                        IconUrl = n.IconUrl,
                        IconActionUrl = n.IconActionUrl,
                        IsRead = n.IsRead,
                    });

                    await dbContext.Notifications.AddRangeAsync(notificationEntities, stoppingToken);
                    await dbContext.SaveChangesAsync(stoppingToken);

                    var hub = scope.ServiceProvider
                        .GetRequiredService<IHubContext<NotificationsHub, INotificationClient>>();

                    foreach (var notification in notificationEntities)
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
                            RecipientId = notification.RecipientId,
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing notification message");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Notification processing cancelled");
        }
    }
}
```

## IPC Models

### Inter-Process Communication Messages

```csharp
namespace ExpertBridge.Notifications.Models.IPC;

public class SendNotificationsRequestMessage
{
    public List<Notification> Notifications { get; set; } = [];
}
```

## Response Models

### Notification Response DTO

```csharp
namespace ExpertBridge.Notifications.Models;

public class NotificationResponse
{
    public string Id { get; set; }
    public string RecipientId { get; set; }
    public string Message { get; set; }
    public string? ActionUrl { get; set; }
    public string? IconUrl { get; set; }
    public string? IconActionUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Message Model

```csharp
namespace ExpertBridge.Notifications.Models;

public class Message
{
    public string Id { get; set; }
    public string Content { get; set; }
    public string SenderId { get; set; }
    public string RecipientId { get; set; }
    public DateTime SentAt { get; set; }
}
```

## Service Registration

### Extension Method for Notification Services

```csharp
namespace ExpertBridge.Notifications.Extensions;

public static class NotificationExtensions
{
    public static IServiceCollection AddNotificationServices(this IServiceCollection services)
    {
        // Register unbounded channel for notification processing
        services.AddSingleton(Channel.CreateUnbounded<SendNotificationsRequestMessage>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            }));

        // Register notification facade
        services.AddScoped<NotificationFacade>();

        // Register background worker
        services.AddHostedService<NotificationSendingPipelineHandlerWorker>();

        // Register SignalR
        services.AddSignalR();

        return services;
    }
}
```

## Client Usage Pattern

### JavaScript/TypeScript Client

```typescript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/api/notificationsHub")
    .withAutomaticReconnect()
    .build();

// Listen for notifications
connection.on("ReceiveNotification", (notification) => {
    console.log("New notification:", notification);
    // Update UI with notification
});

// Listen for messages
connection.on("ReceiveMessage", (message) => {
    console.log("New message:", message);
    // Update UI with message
});

// Start connection
await connection.start();

// Get connection ID (if needed)
const connectionId = await connection.invoke("GetConnectionId");
```

## Best Practices

1. **Use strongly-typed client interfaces** - Compile-time safety for client method calls
2. **Use Channel for background processing** - Efficient, asynchronous message queue
3. **Create scopes in background services** - Proper DI lifetime management
4. **Use IHubContext for external notifications** - Send from services outside hub
5. **Handle connection lifecycle** - Override OnConnectedAsync/OnDisconnectedAsync
6. **Log all operations** - Structured logging for debugging real-time issues
7. **Use proper CORS policy** - SignalR requires AllowCredentials for authenticated connections
8. **Persist notifications** - Store in database before sending to ensure delivery
9. **Use ArgumentNullException.ThrowIfNull** - Validate facade parameters
10. **Batch notifications** - Send multiple notifications in single channel message

## Targeting Clients

### Send to Specific Users

```csharp
// Send to specific user
await hub.Clients.User(userId).ReceiveNotification(notification);

// Send to specific connection
await hub.Clients.Client(connectionId).ReceiveNotification(notification);

// Send to all except caller
await hub.Clients.Others.ReceiveNotification(notification);

// Send to group
await hub.Clients.Group(groupName).ReceiveNotification(notification);

// Send to all
await hub.Clients.All.ReceiveNotification(notification);
```

### Managing Groups

```csharp
public class NotificationsHub : Hub<INotificationClient>
{
    public async Task JoinUserGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
    }

    public async Task LeaveUserGroup(string userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
    }
}
```

## Anti-Patterns to Avoid

-   ❌ Don't perform long-running operations in hub methods
-   ❌ Don't use properties for remotely callable methods (use methods only)
-   ❌ Don't forget to handle disconnections gracefully
-   ❌ Don't send large payloads over SignalR (use references/IDs instead)
-   ❌ Don't forget CORS configuration with AllowCredentials for SignalR
-   ❌ Don't create scoped services in singleton background workers without proper scoping
-   ❌ Don't swallow exceptions in background workers without logging
-   ❌ Don't forget to persist notifications before sending (durability)
-   ❌ Don't use synchronous methods in SignalR hubs
-   ❌ Don't expose sensitive data in notification messages

## Error Handling

```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    try
    {
        while (await _channel.WaitToReadAsync(stoppingToken))
        {
            var message = await _channel.ReadAsync(stoppingToken);

            try
            {
                // Process message
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing notification message: {Message}", message);
                // Don't throw - continue processing other messages
            }
        }
    }
    catch (OperationCanceledException)
    {
        _logger.LogInformation("Notification processing cancelled");
    }
    catch (Exception ex)
    {
        _logger.LogCritical(ex, "Critical error in notification processing");
        throw; // Re-throw critical errors
    }
}
```

## Performance Considerations

1. **Use unbounded channels** - For high-throughput scenarios
2. **Batch database operations** - AddRangeAsync instead of multiple AddAsync
3. **Target specific clients** - Avoid broadcasting to all when possible
4. **Use connection ID mapping** - Track user-to-connection mapping for efficient targeting
5. **Configure SignalR transport** - Consider WebSockets only for best performance
6. **Implement backpressure** - Handle slow consumers gracefully
7. **Use efficient serialization** - System.Text.Json by default
8. **Monitor hub connections** - Track active connections and performance metrics
