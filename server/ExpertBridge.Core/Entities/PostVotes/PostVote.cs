// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.PostVotes;

/// <summary>
/// Represents a user's vote (upvote or downvote) on a post.
/// </summary>
/// <remarks>
/// Post votes help surface quality content and engage the community.
/// Users can only cast one vote per post.
/// </remarks>
public class PostVote : BaseModel
{
    /// <summary>
    /// Gets or sets a value indicating whether this is an upvote (true) or downvote (false).
    /// </summary>
    public bool IsUpvote { get; set; }

    // Foreign keys
    /// <summary>
    /// Gets or sets the unique identifier of the profile that cast the vote.
    /// </summary>
    public string ProfileId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the post being voted on.
    /// </summary>
    public string PostId { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the profile that cast this vote.
    /// </summary>
    public Profile Profile { get; set; }

    /// <summary>
    /// Gets or sets the post that received this vote.
    /// </summary>
    public Post Post { get; set; }
}
