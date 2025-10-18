// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

/// <summary>
/// Represents the response DTO for job posting information.
/// </summary>
/// <remarks>
/// This DTO contains job advertisement details with engagement metrics, budget, and AI-generated tags.
/// Used in job listings, search results, and detail views.
/// </remarks>
public class JobPostingResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the job posting.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Gets or sets the title of the job posting.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the detailed description of the job.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Gets or sets the detected language of the job posting content.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets the author (hirer) information.
    /// </summary>
    public AuthorResponse? Author { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the job posting was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the job posting was last modified.
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// Gets or sets the number of upvotes the job posting has received.
    /// </summary>
    public int Upvotes { get; set; }

    /// <summary>
    /// Gets or sets the number of downvotes the job posting has received.
    /// </summary>
    public int Downvotes { get; set; }

    /// <summary>
    /// Gets or sets the total number of comments on the job posting.
    /// </summary>
    public int Comments { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current user has upvoted this job posting.
    /// </summary>
    public bool IsUpvoted { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current user has downvoted this job posting.
    /// </summary>
    public bool IsDownvoted { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current user has applied for this job.
    /// </summary>
    public bool IsAppliedFor { get; set; }

    /// <summary>
    /// Gets or sets the relevance score for search results or recommendations.
    /// </summary>
    /// <remarks>
    /// This score is calculated using semantic similarity from vector embeddings when returning search or recommendation results.
    /// </remarks>
    public double? RelevanceScore { get; set; }

    /// <summary>
    /// Gets or sets the budget allocated for the job.
    /// </summary>
    public decimal Budget { get; set; }

    /// <summary>
    /// Gets or sets the geographical area or work location for the job.
    /// </summary>
    public string Area { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of AI-generated tags categorizing the job posting.
    /// </summary>
    public List<TagResponse>? Tags { get; set; }

    /// <summary>
    /// Gets or sets the collection of media attachments.
    /// </summary>
    public List<MediaObjectResponse>? Medias { get; set; }
}
