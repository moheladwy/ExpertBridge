// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ChatParticipants;
using ExpertBridge.Api.Core.Entities.Media.ChatMedia;

namespace ExpertBridge.Api.Core.Entities.Chats;

public class Chat
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }

    // Navigation properties
    public ICollection<ChatParticipant> Participants { get; set; } = [];
    public ICollection<ChatMedia> Medias { get; set; } = [];
}
