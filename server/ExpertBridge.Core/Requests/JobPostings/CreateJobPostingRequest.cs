// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace ExpertBridge.Core.Requests.JobPostings;

/// <summary>
/// Represents a request to create a new job posting.
/// </summary>
/// <remarks>
/// Job postings are automatically processed by AI services for language detection, tagging,
/// content moderation, and embedding generation for semantic job matching.
/// </remarks>
public class CreateJobPostingRequest
{
    /// <summary>
    /// Gets or sets the geographical area or work location for the job.
    /// </summary>
    public required string Area { get; set; }

    /// <summary>
    /// Gets or sets the title of the job posting.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the detailed description of the job.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Gets or sets the budget allocated for the job.
    /// </summary>
    /// <remarks>
    /// Must be a non-negative value.
    /// </remarks>
    [Range(0, double.MaxValue, ErrorMessage = "Budget must be non-negative.")]
    public decimal Budget { get; set; }

    /// <summary>
    /// Gets or sets the collection of media attachments for the job posting.
    /// </summary>
    public List<MediaObjectRequest>? Media { get; set; }
}
