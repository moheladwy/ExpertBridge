// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.SearchUser;

/// <summary>
/// Represents a request to search for users using semantic similarity.
/// </summary>
/// <remarks>
/// Search uses vector embeddings to find user profiles semantically similar to the query,
/// enabling discovery of experts based on skills, interests, and professional background.
/// </remarks>
public class SearchUserRequest
{
    /// <summary>
    /// Gets or sets the search query text.
    /// </summary>
    /// <remarks>
    /// The query is converted to an embedding vector and compared against user interest embeddings
    /// using cosine similarity to find the most relevant profiles.
    /// </remarks>
    public required string Query { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of search results to return.
    /// </summary>
    public int? Limit { get; set; }
}
