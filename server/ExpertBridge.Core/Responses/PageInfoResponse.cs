// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

/// <summary>
/// Represents pagination metadata for cursor-based pagination.
/// </summary>
/// <remarks>
/// This DTO supports both ID-based cursors for standard pagination and vector embedding-based cursors
/// for semantic similarity searches. Used with paginated list responses.
/// </remarks>
public class PageInfoResponse
{
    /// <summary>
    /// Gets or sets the ID cursor for the next page of results.
    /// </summary>
    /// <remarks>
    /// This is used for standard ID-based pagination. The value represents the last item ID from the current page.
    /// </remarks>
    public string? NextIdCursor { get; set; }

    /// <summary>
    /// Gets or sets the similarity score cursor for the next page of semantic search results.
    /// </summary>
    /// <remarks>
    /// This is used for vector embedding-based pagination where results are ordered by similarity score.
    /// The value represents the lowest similarity score from the current page.
    /// </remarks>
    public double? EndCursor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether there are more pages available after the current page.
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Gets or sets the base64-encoded embedding vector used for similarity searches.
    /// </summary>
    /// <remarks>
    /// This embedding is returned with search results and can be reused for subsequent pagination requests
    /// to maintain consistency in similarity calculations.
    /// </remarks>
    public string? Embedding { get; set; }
}
