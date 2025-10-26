// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

/// <summary>
/// Represents the response DTO for job offer information.
/// </summary>
/// <remarks>
/// This DTO contains details about a direct job offer from a hirer to a specific worker,
/// distinct from job postings which are open to multiple applicants.
/// </remarks>
public class JobOfferResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the job offer.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the job offer.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the detailed description of the job offer.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets the budget offered for the job.
    /// </summary>
    public decimal Budget { get; set; }

    /// <summary>
    /// Gets or sets the geographical area or work location for the job.
    /// </summary>
    public required string Area { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the job offer was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the author (hirer) information.
    /// </summary>
    public AuthorResponse? Author { get; set; }

    /// <summary>
    /// Gets or sets the worker to whom the offer is extended.
    /// </summary>
    public AuthorResponse? Worker { get; set; }
}
