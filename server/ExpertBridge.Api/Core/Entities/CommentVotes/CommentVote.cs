// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Core.Entities.CommentVotes;

public class CommentVote
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public bool IsUpvote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign keys
    public string CommentId { get; set; }
    public string ProfileId { get; set; }

    // Navigation properties
    public Profiles.Profile Profile { get; set; }
    public Comments.Comment Comment { get; set; }
}
