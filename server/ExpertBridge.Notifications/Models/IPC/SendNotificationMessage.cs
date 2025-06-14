// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Notifications.Models.IPC
{
    public class SendNotificationMessage
    {
        public string RecipientId { get; set; } // The profile ID of the recipient
        public string SenderId { get; set; } // The profile ID of the sender
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string? ActionUrl { get; set; }
        public string? IconUrl { get; set; } // The URL of the icon associated with the notification
        public string? IconActionUrl { get; set; } // The action URL associated with clicking media (e.g. sender user profile)
    }
}
