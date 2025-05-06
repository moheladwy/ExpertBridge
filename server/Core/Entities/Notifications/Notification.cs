using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Notifications
{
    // This model is for representing the notification object
    // used inside the context of this project (Notifications project).
    // It should have the same shape as the NotificationResponse returned
    // from the NotificationsController.

    public class Notification : BaseModel
    {
        /// <summary>
        /// The profile ID of the recipient
        /// </summary>
        public string RecipientId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string? ActionUrl { get; set; }
        public string? IconUrl { get; set; } // The URL of the icon associated with the notification
        public string? IconActionUrl { get; set; } // The action URL associated with clicking media (e.g. sender user profile)
    }
}
