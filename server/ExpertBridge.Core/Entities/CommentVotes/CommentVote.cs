// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.CommentVotes;

/// <summary>
///     Represents a user's vote (upvote or downvote) on a comment.
/// </summary>
/// <remarks>
///     Comment votes contribute to a user's reputation score and help surface quality content.
///     Users can only cast one vote per comment.
/// </remarks>
public class CommentVote : BaseModel
{
    /// <summary>
    ///     Gets or sets a value indicating whether this is an upvote (true) or downvote (false).
    /// </summary>
    public bool IsUpvote { get; set; }

    // Foreign keys
    /// <summary>
    ///     Gets or sets the unique identifier of the comment being voted on.
    /// </summary>
    public required string CommentId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the profile that cast the vote.
    /// </summary>
    public required string ProfileId { get; set; }

    // Navigation properties
    /// <summary>
    ///     Gets or sets the profile that cast this vote.
    /// </summary>
    public Profile Profile { get; set; }

    /// <summary>
    ///     Gets or sets the comment that received this vote.
    /// </summary>
    public Comment Comment { get; set; }
}
