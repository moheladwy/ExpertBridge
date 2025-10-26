// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

/// <summary>
///     Represents the response DTO for comment information.
/// </summary>
/// <remarks>
///     This record supports threaded discussions with nested replies and includes engagement metrics.
///     Comments can be associated with either posts or job postings.
/// </remarks>
public record CommentResponse
{
    /// <summary>
    ///     Gets or sets the unique identifier of the comment.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the author information.
    /// </summary>
    public AuthorResponse? Author { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the comment author.
    /// </summary>
    public required string AuthorId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the post this comment belongs to.
    /// </summary>
    /// <remarks>
    ///     Either <see cref="PostId" /> or <see cref="JobPostingId" /> will be set, but not both.
    /// </remarks>
    public string? PostId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the job posting this comment belongs to.
    /// </summary>
    /// <remarks>
    ///     Either <see cref="PostId" /> or <see cref="JobPostingId" /> will be set, but not both.
    /// </remarks>
    public string? JobPostingId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the parent comment for nested replies.
    /// </summary>
    /// <remarks>
    ///     This property is null for top-level comments.
    /// </remarks>
    public string? ParentCommentId { get; set; }

    /// <summary>
    ///     Gets or sets the text content of the comment.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the comment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the comment was last modified.
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    ///     Gets or sets the collection of nested reply comments.
    /// </summary>
    public List<CommentResponse> Replies { get; set; } = [];

    /// <summary>
    ///     Gets or sets the number of upvotes the comment has received.
    /// </summary>
    public int Upvotes { get; set; }

    /// <summary>
    ///     Gets or sets the number of downvotes the comment has received.
    /// </summary>
    public int Downvotes { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the current user has upvoted this comment.
    /// </summary>
    public bool IsUpvoted { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the current user has downvoted this comment.
    /// </summary>
    public bool IsDownvoted { get; set; }

    /// <summary>
    ///     Gets or sets the collection of media attachments.
    /// </summary>
    public List<MediaObjectResponse>? Medias { get; set; }
}
