// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Contract.Responses;

/// <summary>
///     Represents the response DTO for similar job posting recommendations.
/// </summary>
/// <remarks>
///     This DTO is used to return job postings that are semantically similar to a given job posting,
///     calculated using vector embeddings and cosine similarity. Used for "related jobs" features.
/// </remarks>
public sealed class SimilarJobsResponse
{
    /// <summary>
    ///     Gets or sets the unique identifier of the similar job posting.
    /// </summary>
    public string JobPostingId { get; set; }

    /// <summary>
    ///     Gets or sets the title of the similar job posting.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     Gets or sets the content of the similar job posting.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    ///     Gets or sets the name of the job posting author (hirer).
    /// </summary>
    public string AuthorName { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the job posting was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the budget allocated for the job.
    /// </summary>
    public decimal Budget { get; set; }

    /// <summary>
    ///     Gets or sets the geographical area or work location for the job.
    /// </summary>
    public string Area { get; set; }

    /// <summary>
    ///     Gets or sets the semantic similarity score to the reference job posting.
    /// </summary>
    /// <remarks>
    ///     This score is calculated using cosine similarity between embedding vectors.
    ///     Higher scores indicate greater semantic similarity.
    /// </remarks>
    public double RelevanceScore { get; set; }
}
