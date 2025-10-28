// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace ExpertBridge.Contract.Requests.CreateJobOffer;

/// <summary>
///     Represents a request to create a direct job offer to a specific worker.
/// </summary>
/// <remarks>
///     Job offers are direct invitations from hirers to workers, distinct from open job postings
///     that accept applications from multiple candidates.
/// </remarks>
public class CreateJobOfferRequest
{
    /// <summary>
    ///     Gets or sets the title of the job offer.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    ///     Gets or sets the detailed description of the job offer.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    ///     Gets or sets the geographical area or work location for the job.
    /// </summary>
    public required string Area { get; set; }

    /// <summary>
    ///     Gets or sets the budget offered for the job.
    /// </summary>
    /// <remarks>
    ///     Must be a non-negative value.
    /// </remarks>
    [Range(0, double.MaxValue, ErrorMessage = "Budget must be non-negative.")]
    public decimal Budget { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the worker receiving the job offer.
    /// </summary>
    public string WorkerId { get; set; }
}
