// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.JobReviews;

/// <summary>
/// Represents a review and rating left after a job is completed.
/// </summary>
/// <remarks>
/// Job reviews allow both hirers and workers to rate and provide feedback on their collaboration experience.
/// Reviews contribute to profile reputation and help build trust in the platform.
/// </remarks>
public class JobReview : BaseModel
{
    /// <summary>
    /// Gets or sets the review text content.
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// Gets or sets the rating given (typically 0-5 stars).
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the review has been deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    // Foreign keys
    /// <summary>
    /// Gets or sets the unique identifier of the worker being reviewed.
    /// </summary>
    public string WorkerId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the customer (hirer) who wrote the review.
    /// </summary>
    public string CustomerId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the job being reviewed.
    /// </summary>
    public string JobId { get; set; } = null!;

    // Navigation properties
    /// <summary>
    /// Gets or sets the worker profile being reviewed.
    /// </summary>
    public Profile Worker { get; set; } = null!;

    /// <summary>
    /// Gets or sets the customer profile who wrote the review.
    /// </summary>
    public Profile Customer { get; set; } = null!;

    /// <summary>
    /// Gets or sets the job contract this review is about.
    /// </summary>
    public Job Job { get; set; } = null!;
}
