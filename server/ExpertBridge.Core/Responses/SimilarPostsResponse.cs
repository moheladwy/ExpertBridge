// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

/// <summary>
/// Represents the response DTO for similar post recommendations.
/// </summary>
/// <remarks>
/// This DTO is used to return posts that are semantically similar to a given post,
/// calculated using vector embeddings and cosine similarity. Used for "related posts" features.
/// </remarks>
public class SimilarPostsResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the similar post.
    /// </summary>
    public string PostId { get; set; }

    /// <summary>
    /// Gets or sets the title of the similar post.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the content of the similar post.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Gets or sets the name of the post author.
    /// </summary>
    public string AuthorName { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the post was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the semantic similarity score to the reference post.
    /// </summary>
    /// <remarks>
    /// This score is calculated using cosine similarity between embedding vectors.
    /// Higher scores indicate greater semantic similarity.
    /// </remarks>
    public double RelevanceScore { get; set; }
}
