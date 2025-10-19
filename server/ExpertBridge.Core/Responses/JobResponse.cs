// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobStatuses;

namespace ExpertBridge.Core.Responses;

/// <summary>
/// Represents the response DTO for active job information.
/// </summary>
/// <remarks>
/// This DTO contains details about an active job contract between a hirer and worker,
/// including status, payment information, and timeline.
/// </remarks>
public class JobResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the job.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the job.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the detailed description of the job.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the associated chat conversation.
    /// </summary>
    public string ChatId { get; set; }

    /// <summary>
    /// Gets or sets the actual cost agreed upon for the job.
    /// </summary>
    public decimal ActualCost { get; set; }

    /// <summary>
    /// Gets or sets the geographical area or work location for the job.
    /// </summary>
    public string Area { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the job was started.
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the job was completed.
    /// </summary>
    public DateTime? EndedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether payment has been processed for the job.
    /// </summary>
    public bool IsPaid { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the job has been marked as completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Gets or sets the current status of the job.
    /// </summary>
    /// <remarks>
    /// Status values include: Pending, InProgress, Completed, Cancelled, etc.
    /// </remarks>
    public JobStatusEnum Status { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the job status was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the hirer who created the job.
    /// </summary>
    public required string AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the worker assigned to the job.
    /// </summary>
    public required string WorkerId { get; set; }

    /// <summary>
    /// Gets or sets the author (hirer) information.
    /// </summary>
    public AuthorResponse Author { get; set; }

    /// <summary>
    /// Gets or sets the worker information.
    /// </summary>
    public AuthorResponse Worker { get; set; }
}
