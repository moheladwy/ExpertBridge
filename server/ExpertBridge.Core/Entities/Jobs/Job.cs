// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Chats;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.JobReviews;
using ExpertBridge.Core.Entities.JobStatuses;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.Jobs;

/// <summary>
///     Represents an active job contract between a hirer and a worker.
/// </summary>
/// <remarks>
///     A Job entity is created when a job offer is accepted, establishing a contractual agreement between parties.
///     It tracks the job lifecycle from start to completion, including payment status, reviews, and associated chat
///     communication.
/// </remarks>
public sealed class Job : BaseModel, ISoftDeletable
{
    /// <summary>
    ///     Gets or sets the title of the job contract.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    ///     Gets or sets the detailed description of the job requirements and deliverables.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the chat conversation for this job.
    /// </summary>
    public string ChatId { get; set; }

    /// <summary>
    ///     Gets or sets the actual cost paid for the completed job.
    /// </summary>
    /// <remarks>
    ///     This may differ from the initial budget if renegotiated or adjusted for partial completion.
    /// </remarks>
    public decimal ActualCost { get; set; }

    /// <summary>
    ///     Gets or sets the geographic area or location where the job is performed.
    /// </summary>
    public string Area { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when work on the job commenced.
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the job was completed.
    /// </summary>
    public DateTime? EndedAt { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether payment has been processed for the job.
    /// </summary>
    public bool IsPaid { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the job has been completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    ///     Gets or sets the current status of the job in its lifecycle.
    /// </summary>
    /// <remarks>
    ///     The status tracks progression through states such as Active, InProgress, Completed, Cancelled, etc.
    /// </remarks>
    public JobStatusEnum Status { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the job details were last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the job author (hirer).
    /// </summary>
    public required string AuthorId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the worker assigned to the job.
    /// </summary>
    public required string WorkerId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the review written after job completion.
    /// </summary>
    public string? ReviewId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the job posting this job originated from.
    /// </summary>
    /// <remarks>
    ///     This property is null if the job was created from a direct job offer rather than a posting.
    /// </remarks>
    public string? JobPostingId { get; set; }

    /// <summary>
    ///     Gets or sets the review written for this job after completion.
    /// </summary>
    public JobReview? Review { get; set; }

    /// <summary>
    ///     Gets or sets the job posting this job originated from.
    /// </summary>
    public JobPosting? JobPosting { get; set; }

    /// <summary>
    ///     Gets or sets the profile of the job author (hirer).
    /// </summary>
    public Profile Author { get; set; }

    /// <summary>
    ///     Gets or sets the profile of the assigned worker.
    /// </summary>
    public Profile Worker { get; set; }

    /// <summary>
    ///     Gets or sets the chat conversation associated with this job.
    /// </summary>
    public Chat Chat { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the job is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the job was marked as deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
