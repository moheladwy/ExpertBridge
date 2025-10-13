// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.Messages;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.Chats;

public class Chat : BaseModel, ISoftDeletable
{
    public required string HirerId { get; set; }
    public required string WorkerId { get; set; }
    public DateTime? EndedAt { get; set; }

    // Navigation properties
    public Profile Hirer { get; set; }
    public Profile Worker { get; set; }
    public Job Job { get; set; }

    public ICollection<Message> Messages { get; set; } = [];

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    //public ICollection<ChatParticipant> Participants { get; set; } = [];
    //public ICollection<ChatMedia> Medias { get; set; } = [];
}
