// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.Notifications;
// This model is for representing the notification object
// used inside the context of this project (Notifications project).
// It should have the same shape as the NotificationResponse returned
// from the NotificationsController.

public class Notification : BaseModel
{
    /// <summary>
    ///     The profile ID of the recipient
    /// </summary>
    public string RecipientId { get; set; }

    public string? SenderId { get; set; } // The profile ID of the action creator
    public string Message { get; set; }
    public bool IsRead { get; set; } = false;
    public string? ActionUrl { get; set; }
    public string? IconUrl { get; set; } // The URL of the icon associated with the notification

    public string?
        IconActionUrl { get; set; } // The action URL associated with clicking media (e.g. sender user profile)
}
