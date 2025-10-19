// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.EditComment;

/// <summary>
/// Represents a request to partially update a comment with voting or content changes.
/// </summary>
/// <remarks>
/// This request supports voting operations and content updates in a single call.
/// All properties except CommentId are optional.
/// </remarks>
public class PatchCommentRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the comment to update.
    /// </summary>
    public required string CommentId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to upvote the comment.
    /// </summary>
    /// <remarks>
    /// Set to true to upvote, false to remove upvote, or null to leave unchanged.
    /// </remarks>
    public bool? Upvote { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to downvote the comment.
    /// </summary>
    /// <remarks>
    /// Set to true to downvote, false to remove downvote, or null to leave unchanged.
    /// </remarks>
    public bool? Downvote { get; set; }

    /// <summary>
    /// Gets or sets the new text content for the comment.
    /// </summary>
    public string? Content { get; set; }
}
