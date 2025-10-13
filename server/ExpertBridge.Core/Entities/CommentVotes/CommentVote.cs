// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.CommentVotes;

public class CommentVote : BaseModel
{
    public bool IsUpvote { get; set; }

    // Foreign keys
    public required string CommentId { get; set; }
    public required string ProfileId { get; set; }

    // Navigation properties
    public Profile Profile { get; set; }
    public Comment Comment { get; set; }
}
