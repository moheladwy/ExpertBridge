// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.ManyToManyRelationships.ChatParticipants;
using ExpertBridge.Core.Entities.Media.ChatMedia;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.Chats;

public class Chat : BaseModel, ISoftDeletable
{
    public DateTime? EndedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public ICollection<ChatParticipant> Participants { get; set; } = [];
    public ICollection<ChatMedia> Medias { get; set; } = [];
}
