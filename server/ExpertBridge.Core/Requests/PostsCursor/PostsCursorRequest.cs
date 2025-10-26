// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.PostsCursor;

/// <summary>
/// Represents a cursor-based pagination request for post listings.
/// </summary>
/// <remarks>
/// Supports both standard chronological pagination and semantic similarity-based pagination
/// using vector embeddings for personalized content recommendations.
/// </remarks>
public class PostsCursorRequest
{
    /// <summary>
    /// Gets or sets the number of posts to return per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the page number (1-based).
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the similarity score cursor for continuing semantic search pagination.
    /// </summary>
    /// <remarks>
    /// This represents the lowest similarity score from the previous page and is used
    /// when paginating through embedding-based search results.
    /// </remarks>
    public double? After { get; set; }

    /// <summary>
    /// Gets or sets the ID cursor for continuing standard pagination.
    /// </summary>
    /// <remarks>
    /// This represents the ID of the last post from the previous page.
    /// </remarks>
    public string? LastIdCursor { get; set; }

    /// <summary>
    /// Gets or sets the base64-encoded embedding vector for semantic similarity searches.
    /// </summary>
    /// <remarks>
    /// When provided, posts are ordered by semantic similarity to this embedding
    /// rather than chronologically. This is used for personalized content recommendations.
    /// </remarks>
    public string? Embedding { get; set; }
}
