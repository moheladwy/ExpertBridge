using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertBridge.Notifications.Models
{
    /// <summary>
    /// Techincally, this is the Core.NotificationResponse model.
    /// But we don't want to couple it with that model so we have more
    /// flixibility in the future in case of changes to the notifications system
    /// or it being used by other systems. 
    /// </summary>
    public class Notification
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RecipientId { get; set; } // The profile ID of the recipient
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string? ActionUrl { get; set; }
        public string? IconUrl { get; set; } // The URL of the icon associated with the notification
        public string? IconActionUrl { get; set; } // The action URL associated with clicking media (e.g. sender user profile)
    }
}
