// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

/// <summary>
/// Represents the response DTO for post information.
/// </summary>
/// <remarks>
/// This record contains complete post details including content, author, engagement metrics,
/// media attachments, and AI-generated tags. Used in post listings and detail views.
/// </remarks>
public record PostResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the post.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Gets or sets the title of the post.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the main content body of the post.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Gets or sets the detected language of the post content.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets the author information.
    /// </summary>
    public AuthorResponse? Author { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the post was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the post was last modified.
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// Gets or sets the number of upvotes the post has received.
    /// </summary>
    public int Upvotes { get; set; }

    /// <summary>
    /// Gets or sets the number of downvotes the post has received.
    /// </summary>
    public int Downvotes { get; set; }

    /// <summary>
    /// Gets or sets the total number of comments on the post.
    /// </summary>
    public int Comments { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current user has upvoted this post.
    /// </summary>
    public bool IsUpvoted { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current user has downvoted this post.
    /// </summary>
    public bool IsDownvoted { get; set; }

    /// <summary>
    /// Gets or sets the relevance score for search results or recommendations.
    /// </summary>
    /// <remarks>
    /// This score is calculated using semantic similarity from vector embeddings when returning search or recommendation results.
    /// </remarks>
    public double? RelevanceScore { get; set; }

    /// <summary>
    /// Gets or sets the collection of media attachments.
    /// </summary>
    public List<MediaObjectResponse>? Medias { get; set; }

    /// <summary>
    /// Gets or sets the collection of AI-generated tags categorizing the post.
    /// </summary>
    public List<TagResponse>? Tags { get; set; }
}
