// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Notifications.Models;

/// <summary>
///     Represents a notification transmitted via SignalR to connected clients for real-time notification delivery.
///     Decoupled from the Core.Entities.Notification persistence model for architectural flexibility.
/// </summary>
/// <remarks>
///     This DTO is specifically designed for SignalR communication through the
///     <see cref="INotificationClient.ReceiveNotification" /> method.
///     While structurally similar to the database entity, maintaining separation provides important benefits:
///     **Architectural Benefits:**
///     - Flexibility to evolve notification delivery format independently from database schema
///     - Prevents tight coupling between real-time communication and persistence layers
///     - Enables different notification systems to consume without Core.Entities dependencies
///     - Supports future changes to notification infrastructure without affecting domain models
///     **Property Details:**
///     - Id: Unique identifier for the notification (from database entity)
///     - CreatedAt: Timestamp when notification was created
///     - RecipientId: Profile ID of the user receiving the notification
///     - Message: Human-readable notification text displayed to the user
///     - IsRead: Indicates whether the user has viewed the notification
///     - ActionUrl: Optional URL for navigation when user clicks the notification
///     - IconUrl: Optional URL for an icon/avatar displayed with the notification (e.g., sender's profile picture)
///     - IconActionUrl: Optional URL for navigation when user clicks the icon (e.g., sender's profile page)
///     **Use Cases:**
///     - Real-time push notifications for platform events (comments, votes, job applications)
///     - In-app notification center updates
///     - Badge count synchronization across client sessions
///     Transformed from Core.Entities.Notification by NotificationSendingPipelineHandlerWorker before broadcasting.
/// </remarks>
public sealed class NotificationResponse
{
    /// <summary>
    ///     Gets or sets the unique identifier for the notification.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///     Gets or sets the timestamp when the notification was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the profile ID of the recipient user.
    /// </summary>
    public string RecipientId { get; set; }

    /// <summary>
    ///     Gets or sets the human-readable notification message displayed to the user.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the notification has been read by the recipient.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    ///     Gets or sets the optional URL for navigation when the user clicks the notification.
    /// </summary>
    /// <remarks>
    ///     Examples: post detail page, comment thread, job application details, etc.
    /// </remarks>
    public string? ActionUrl { get; set; }

    /// <summary>
    ///     Gets or sets the optional URL for an icon or avatar displayed with the notification.
    /// </summary>
    /// <remarks>
    ///     Typically the profile picture or content thumbnail of the notification trigger (e.g., user who commented).
    /// </remarks>
    public string? IconUrl { get; set; }

    /// <summary>
    ///     Gets or sets the optional URL for navigation when the user clicks the notification icon.
    /// </summary>
    /// <remarks>
    ///     Typically links to the profile page of the user who triggered the notification.
    /// </remarks>
    public string? IconActionUrl { get; set; }
}
