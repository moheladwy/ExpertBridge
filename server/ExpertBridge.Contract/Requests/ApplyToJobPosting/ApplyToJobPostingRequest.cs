// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace ExpertBridge.Contract.Requests.ApplyToJobPosting;

/// <summary>
///     Represents a request to apply for a job posting.
/// </summary>
/// <remarks>
///     Workers submit applications with their proposed cost and an optional cover letter
///     to express interest in a job posting.
/// </remarks>
public sealed class ApplyToJobPostingRequest
{
    /// <summary>
    ///     Gets or sets the unique identifier of the job posting to apply for.
    /// </summary>
    public required string JobPostingId { get; set; }

    /// <summary>
    ///     Gets or sets the cover letter for the application.
    /// </summary>
    /// <remarks>
    ///     Optional but recommended to explain qualifications and interest.
    /// </remarks>
    public string? CoverLetter { get; set; }

    /// <summary>
    ///     Gets or sets the cost proposed by the applicant for completing the job.
    /// </summary>
    /// <remarks>
    ///     Must be a non-negative value.
    /// </remarks>
    [Range(0, double.MaxValue, ErrorMessage = "OfferedCost must be non-negative.")]
    public required decimal OfferedCost { get; set; }
}
