// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Contract.Requests.SearchPost;

/// <summary>
///     Represents a request to search for posts using semantic similarity.
/// </summary>
/// <remarks>
///     Search uses vector embeddings to find posts semantically similar to the query,
///     providing more relevant results than traditional keyword matching.
/// </remarks>
public class SearchPostRequest
{
    /// <summary>
    ///     Gets or sets the search query text.
    /// </summary>
    /// <remarks>
    ///     The query is converted to an embedding vector and compared against post embeddings
    ///     using cosine similarity to find the most relevant posts.
    /// </remarks>
    public string Query { get; set; }

    /// <summary>
    ///     Gets or sets the maximum number of search results to return.
    /// </summary>
    public int? Limit { get; set; }
}
