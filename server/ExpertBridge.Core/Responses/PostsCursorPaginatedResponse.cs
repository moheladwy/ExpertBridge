// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

/// <summary>
/// Represents a paginated response containing a list of posts.
/// </summary>
/// <remarks>
/// This DTO wraps post collections with pagination metadata for cursor-based navigation.
/// Supports both standard chronological ordering and semantic similarity-based ordering.
/// </remarks>
public class PostsCursorPaginatedResponse
{
    /// <summary>
    /// Gets or sets the collection of posts for the current page.
    /// </summary>
    public List<PostResponse> Posts { get; set; }

    /// <summary>
    /// Gets or sets the pagination metadata including cursors and page availability.
    /// </summary>
    public PageInfoResponse PageInfo { get; set; }
}
