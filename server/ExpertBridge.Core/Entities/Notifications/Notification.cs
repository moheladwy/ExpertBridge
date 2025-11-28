// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.Notifications;

/// <summary>
///     Represents a notification sent to a user about platform activity and events.
/// </summary>
/// <remarks>
///     Notifications are delivered in real-time through SignalR and stored for historical reference.
///     They alert users to various events such as comments, votes, job offers, messages, and system updates.
///     This model represents the notification entity used within the Notifications project context and matches
///     the shape of <see cref="Core.Responses.NotificationResponse" /> returned from the API.
/// </remarks>
public sealed class Notification : BaseModel
{
    /// <summary>
    ///     Gets or sets the unique identifier of the notification recipient.
    /// </summary>
    public string RecipientId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the user who triggered the notification.
    /// </summary>
    /// <remarks>
    ///     This property is null for system-generated notifications.
    /// </remarks>
    public string? SenderId { get; set; }

    /// <summary>
    ///     Gets or sets the notification message text.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the notification has been read by the recipient.
    /// </summary>
    public bool IsRead { get; set; } = false;

    /// <summary>
    ///     Gets or sets the URL to navigate to when the notification is clicked.
    /// </summary>
    /// <remarks>
    ///     Examples include links to specific posts, job postings, profiles, or chat conversations.
    /// </remarks>
    public string? ActionUrl { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the icon or avatar to display with the notification.
    /// </summary>
    /// <remarks>
    ///     Typically displays the sender's profile picture or a relevant activity icon.
    /// </remarks>
    public string? IconUrl { get; set; }

    /// <summary>
    ///     Gets or sets the URL to navigate to when the notification icon is clicked.
    /// </summary>
    /// <remarks>
    ///     This is distinct from <see cref="ActionUrl" /> and typically links to the sender's profile.
    /// </remarks>
    public string? IconActionUrl { get; set; }
}
