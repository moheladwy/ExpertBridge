// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

// using System.Numerics;

using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags;
using ExpertBridge.Core.Entities.Media.PostMedia;
using ExpertBridge.Core.Entities.PostVotes;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;
using Pgvector;

namespace ExpertBridge.Core.Entities.Posts;

/// <summary>
/// Represents a user-generated post in the ExpertBridge platform.
/// </summary>
/// <remarks>
/// Posts are the primary content type for knowledge sharing and professional discussions.
/// They support AI-powered features including automatic tagging, content moderation, semantic search through vector embeddings,
/// and are indexed for similarity-based recommendations.
/// </remarks>
public class Post : BaseModel, ISoftDeletable, IRecommendableContent, ISafeContent
{
    /// <summary>
    /// Gets or sets the date and time when the author last updated the post content.
    /// </summary>
    /// <remarks>
    /// This is distinct from <see cref="BaseModel.LastModified"/> which is automatically set by the system for any changes.
    /// </remarks>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the profile of the user who authored the post.
    /// </summary>
    public Profile Author { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of media attachments associated with the post.
    /// </summary>
    public ICollection<PostMedia> Medias { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of comments made on the post.
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of votes (upvotes/downvotes) on the post.
    /// </summary>
    public ICollection<PostVote> Votes { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of tags automatically assigned to the post.
    /// </summary>
    public ICollection<PostTag> PostTags { get; set; } = [];

    /// <summary>
    /// Gets or sets the title of the post.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the main content body of the post.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the post author.
    /// </summary>
    public required string AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the detected language of the post content.
    /// </summary>
    /// <remarks>
    /// This property is populated by AI language detection services during post processing.
    /// </remarks>
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the post has been automatically tagged.
    /// </summary>
    public bool IsTagged { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the post has been processed through the AI pipeline.
    /// </summary>
    /// <remarks>
    /// Processing includes language detection, tagging, embedding generation, and content moderation.
    /// </remarks>
    public bool IsProcessed { get; set; }

    /// <summary>
    /// Gets or sets the vector embedding representation of the post for semantic similarity search.
    /// </summary>
    /// <remarks>
    /// The embedding is generated using AI models (Ollama) and stored using PostgreSQL pgvector extension.
    /// This enables semantic search and intelligent post recommendations.
    /// </remarks>
    public Vector? Embedding { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the post content has been verified as safe and appropriate.
    /// </summary>
    /// <remarks>
    /// Content is processed through AI-powered moderation to detect inappropriate language or harmful content.
    /// </remarks>
    public bool IsSafeContent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the post is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the post was marked as deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
