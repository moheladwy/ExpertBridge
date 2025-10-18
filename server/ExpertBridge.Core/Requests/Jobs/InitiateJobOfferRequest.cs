// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using ExpertBridge.Core.Entities;

namespace ExpertBridge.Core.Requests.Jobs;

/// <summary>
/// Represents a request to initiate a job offer with validation constraints.
/// </summary>
/// <remarks>
/// This request includes validation for title, description length, and proposed rate.
/// Can optionally reference an existing job posting.
/// </remarks>
public class InitiateJobOfferRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the contractor (worker) profile.
    /// </summary>
    [Required]
    public string ContractorProfileId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the title of the job offer.
    /// </summary>
    /// <remarks>
    /// Must not exceed the maximum title length defined in <see cref="GlobalEntitiesConstraints"/>.
    /// </remarks>
    [Required]
    [StringLength(GlobalEntitiesConstraints.MaxTitleLength)]
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets the detailed description of the job offer.
    /// </summary>
    /// <remarks>
    /// Must not exceed the maximum description length defined in <see cref="GlobalEntitiesConstraints"/>.
    /// </remarks>
    [Required]
    [StringLength(GlobalEntitiesConstraints.MaxDescriptionLength)]
    public string Description { get; set; } = null!;

    /// <summary>
    /// Gets or sets the proposed rate for the job.
    /// </summary>
    /// <remarks>
    /// Must be greater than 0.
    /// </remarks>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Proposed rate must be greater than 0")]
    public double ProposedRate { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the related job posting, if applicable.
    /// </summary>
    /// <remarks>
    /// Optional. Used when the job offer is based on an existing job posting.
    /// </remarks>
    public string? JobPostingId { get; set; }
}
