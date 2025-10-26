// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.SearchJobPosts;

/// <summary>
///     Represents a request to search for job postings with filtering options.
/// </summary>
/// <remarks>
///     Search uses vector embeddings for semantic similarity matching and supports
///     filtering by location, budget, and remote work preferences.
/// </remarks>
public class SearchJobPostsRequest
{
    /// <summary>
    ///     Gets or sets the search query text.
    /// </summary>
    /// <remarks>
    ///     The query is converted to an embedding vector and compared against job posting embeddings
    ///     using cosine similarity to find the most relevant jobs.
    /// </remarks>
    public required string Query { get; set; }

    /// <summary>
    ///     Gets or sets the maximum number of search results to return.
    /// </summary>
    /// <remarks>
    ///     Defaults to 25 results if not specified.
    /// </remarks>
    public int? Limit { get; set; } = 25;

    /// <summary>
    ///     Gets or sets the geographical area filter for job locations.
    /// </summary>
    public string? Area { get; set; }

    /// <summary>
    ///     Gets or sets the minimum budget filter for job postings.
    /// </summary>
    public decimal? MinBudget { get; set; }

    /// <summary>
    ///     Gets or sets the maximum budget filter for job postings.
    /// </summary>
    public decimal? MaxBudget { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to filter for remote work opportunities.
    /// </summary>
    /// <remarks>
    ///     Defaults to false (includes both remote and non-remote jobs).
    /// </remarks>
    public bool IsRemote { get; set; } = false;
}
