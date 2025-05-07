using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertBridge.Core.Responses
{
    public class NotificationResponse
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
