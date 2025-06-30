// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.PostVotes;

public class PostVote : BaseModel
{
    public bool IsUpvote { get; set; }

    // Foreign keys
    public string ProfileId { get; set; }
    public string PostId { get; set; }

    // Navigation properties
    public Profile Profile { get; set; }
    public Post Post { get; set; }
}
