// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.JobOffers;

/// <summary>
/// Represents a direct job offer sent from a hirer to a specific worker.
/// </summary>
/// <remarks>
/// Job offers provide an alternative to the job posting/application workflow by allowing hirers to directly
/// invite workers they want to engage with. Workers can accept or decline these offers.
/// </remarks>
public class JobOffer : BaseModel, ISoftDeletable
{
    /// <summary>
    /// Gets or sets the title of the job offer.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the detailed description of the job requirements and expectations.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets the budget offered for the job.
    /// </summary>
    public required decimal Budget { get; set; }

    /// <summary>
    /// Gets or sets the geographic area or location for the job.
    /// </summary>
    public required string Area { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the worker has accepted the offer.
    /// </summary>
    /// <remarks>
    /// When accepted, a Job entity is created to formalize the contract.
    /// </remarks>
    public bool IsAccepted { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the worker has declined the offer.
    /// </summary>
    public bool IsDeclined { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the job offer author (hirer).
    /// </summary>
    public string AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the worker receiving the offer.
    /// </summary>
    public string WorkerId { get; set; }

    /// <summary>
    /// Gets or sets the profile of the offer author (hirer).
    /// </summary>
    public Profile Author { get; set; }

    /// <summary>
    /// Gets or sets the profile of the worker receiving the offer.
    /// </summary>
    public Profile Worker { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the offer is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the offer was marked as deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
