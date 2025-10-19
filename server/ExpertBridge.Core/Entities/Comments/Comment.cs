// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.CommentVotes;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.Media.CommentMedia;
using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.Comments;

/// <summary>
/// Represents a comment on a post or job posting in the ExpertBridge platform.
/// </summary>
/// <remarks>
/// Comments support threaded discussions through parent-child relationships and include AI-powered content moderation.
/// They can be attached to either posts or job postings, and support nested replies.
/// </remarks>
public class Comment : BaseModel, ISoftDeletable, ISafeContent
{
    /// <summary>
    /// Gets or sets the unique identifier of the comment author.
    /// </summary>
    public required string AuthorId { get; set; }

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
    /// Gets or sets the text content of the comment.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the parent comment for nested replies.
    /// </summary>
    /// <remarks>
    /// This property is null for top-level comments.
    /// </remarks>
    public string? ParentCommentId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the comment has been processed through the AI moderation pipeline.
    /// </summary>
    public bool IsProcessed { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the author last updated the comment content.
    /// </summary>
    /// <remarks>
    /// This is distinct from <see cref="BaseModel.LastModified"/> which is automatically set by the system.
    /// </remarks>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the profile of the user who authored the comment.
    /// </summary>
    public Profile Author { get; set; }

    /// <summary>
    /// Gets or sets the post this comment belongs to.
    /// </summary>
    public Post? Post { get; set; }

    /// <summary>
    /// Gets or sets the job posting this comment belongs to.
    /// </summary>
    public JobPosting? JobPosting { get; set; }

    /// <summary>
    /// Gets or sets the parent comment if this is a reply.
    /// </summary>
    public Comment ParentComment { get; set; }

    /// <summary>
    /// Gets or sets the collection of replies to this comment.
    /// </summary>
    public ICollection<Comment> Replies { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of votes (upvotes/downvotes) on the comment.
    /// </summary>
    public ICollection<CommentVote> Votes { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of media attachments associated with the comment.
    /// </summary>
    public ICollection<CommentMedia> Medias { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the comment content has been verified as safe and appropriate.
    /// </summary>
    /// <remarks>
    /// Content is processed through AI-powered moderation to detect inappropriate language.
    /// </remarks>
    public bool IsSafeContent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the comment is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the comment was marked as deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
