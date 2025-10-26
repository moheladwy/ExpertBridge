// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Requests.MediaObject;

namespace ExpertBridge.Core.Requests.CreateComment;

/// <summary>
/// Represents a request to create a new comment.
/// </summary>
/// <remarks>
/// Comments can be attached to posts or job postings, and support nested threading via parent comments.
/// Comments are automatically processed by AI services for content moderation.
/// </remarks>
public class CreateCommentRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the post this comment belongs to.
    /// </summary>
    /// <remarks>
    /// Either <see cref="PostId"/> or <see cref="JobPostingId"/> must be set, but not both.
    /// </remarks>
    public string? PostId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the job posting this comment belongs to.
    /// </summary>
    /// <remarks>
    /// Either <see cref="PostId"/> or <see cref="JobPostingId"/> must be set, but not both.
    /// </remarks>
    public string? JobPostingId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the parent comment for nested replies.
    /// </summary>
    /// <remarks>
    /// Leave null for top-level comments. Set to create a nested reply.
    /// </remarks>
    public string? ParentCommentId { get; set; }

    /// <summary>
    /// Gets or sets the text content of the comment.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Gets or sets the collection of media attachments for the comment.
    /// </summary>
    public List<MediaObjectRequest>? Media { get; set; }
}
