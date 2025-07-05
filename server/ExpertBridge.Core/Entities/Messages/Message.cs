// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertBridge.Core.Entities.Chats;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.Messages
{
    public class Message : BaseModel, ISoftDeletable
    {
        public required string SenderId { get; set; }
        public required string ChatId { get; set; }
        public required string Content { get; set; }
        public bool IsConfirmationMessage { get; set; } // this will be used for different ui rendering in chat

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt {get; set;}

        public Profile Sender { get; set; }
        public Chat Chat { get; set; }
    }
}
