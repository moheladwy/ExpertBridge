using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertBridge.Notifications.Models.IPC
{
    public class SendNotificationsRequestMessage
    {
        public List<SendNotificationMessage> Notifications { get; set; } = new List<SendNotificationMessage>();
    }
}
