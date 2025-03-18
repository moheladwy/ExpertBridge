// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Core.Entities.Votes.PostVote;

public class PostVote
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public bool IsUpvote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign keys
    public string ProfileId { get; set; }
    public string PostId { get; set; }

    // Navigation properties
    public Profiles.Profile Profile { get; set; }
    public Posts.Post Post { get; set; }
}
