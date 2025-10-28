// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Contract.Responses;

/// <summary>
///     Represents the response DTO for notification information.
/// </summary>
/// <remarks>
///     Notifications are delivered in real-time via SignalR and include action links
///     for navigation to relevant content.
/// </remarks>
public class NotificationResponse
{
    /// <summary>
    ///     Gets or sets the unique identifier of the notification.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the notification was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the recipient profile.
    /// </summary>
    public string RecipientId { get; set; }

    /// <summary>
    ///     Gets or sets the notification message text.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the notification has been read by the recipient.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    ///     Gets or sets the URL to navigate to when the notification is clicked.
    /// </summary>
    /// <remarks>
    ///     This typically links to the relevant post, comment, job, or other content that triggered the notification.
    /// </remarks>
    public string? ActionUrl { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the icon to display with the notification.
    /// </summary>
    /// <remarks>
    ///     This is often a profile picture or content-type icon.
    /// </remarks>
    public string? IconUrl { get; set; }

    /// <summary>
    ///     Gets or sets the URL to navigate to when the notification icon is clicked.
    /// </summary>
    /// <remarks>
    ///     This typically links to the sender's profile or the source of the notification.
    /// </remarks>
    public string? IconActionUrl { get; set; }
}
