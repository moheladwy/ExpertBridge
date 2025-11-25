// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Contract.Messages;

/// <summary>
///     Represents a single notification to be queued for processing in the Channel-based notification pipeline.
///     Used for inter-process communication (IPC) between <see cref="NotificationFacade" /> and
///     <see cref="NotificationSendingPipelineHandlerWorker" />.
/// </summary>
/// <remarks>
///     This DTO serves as the contract for notification data transmitted through the unbounded Channel&lt;
///     SendNotificationsRequestMessage&gt;.
///     It carries all necessary information for the background worker to persist the notification and broadcast it via
///     SignalR.
///     **Pipeline Flow:**
///     1. NotificationFacade creates SendNotificationMessage from domain events
///     2. Message is written to Channel (non-blocking operation)
///     3. NotificationSendingPipelineHandlerWorker reads message from Channel
///     4. Worker transforms message into Core.Entities.Notification for persistence
///     5. Worker transforms message into NotificationResponse for SignalR broadcast
///     **Key Properties:**
///     - RecipientId: Profile ID of the user who should receive the notification
///     - SenderId: Profile ID of the user who triggered the notification action
///     - Message: Human-readable text explaining the notification event
///     - IsRead: Initial read status (typically false for new notifications)
///     - ActionUrl: Optional link to related content (post, comment, job, etc.)
///     - IconUrl: Optional sender's profile picture or content thumbnail
///     - IconActionUrl: Optional link to sender's profile when clicking icon
///     **Design Rationale:**
///     - Decouples notification creation from delivery (fire-and-forget pattern)
///     - Enables reliable queueing with backpressure handling
///     - Prevents blocking API operations during notification persistence
///     - Supports batching for efficiency (multiple messages in one request)
///     This is an internal IPC model and should not be exposed via public APIs or stored directly in the database.
/// </remarks>
public sealed class SendNotificationMessage
{
    /// <summary>
    ///     Gets or sets the profile ID of the recipient user who should receive this notification.
    /// </summary>
    public string RecipientId { get; set; }

    /// <summary>
    ///     Gets or sets the profile ID of the sender user who triggered this notification.
    /// </summary>
    public string SenderId { get; set; }

    /// <summary>
    ///     Gets or sets the human-readable notification message text.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the notification should be marked as read initially.
    /// </summary>
    /// <remarks>
    ///     Typically false for new notifications. May be true for system-generated notifications.
    /// </remarks>
    public bool IsRead { get; set; }

    /// <summary>
    ///     Gets or sets the optional URL for navigation when the user clicks the notification.
    /// </summary>
    /// <remarks>
    ///     Examples: /posts/{id}, /comments/{id}, /jobs/{id}/applications, etc.
    /// </remarks>
    public string? ActionUrl { get; set; }

    /// <summary>
    ///     Gets or sets the optional URL for an icon or avatar displayed with the notification.
    /// </summary>
    /// <remarks>
    ///     Typically the sender's profile picture or related content thumbnail.
    /// </remarks>
    public string? IconUrl { get; set; }

    /// <summary>
    ///     Gets or sets the optional URL for navigation when the user clicks the notification icon.
    /// </summary>
    /// <remarks>
    ///     Typically links to the sender's profile page: /profiles/{senderId}
    /// </remarks>
    public string? IconActionUrl { get; set; }
}
