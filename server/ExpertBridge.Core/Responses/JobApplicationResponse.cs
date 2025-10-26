// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

/// <summary>
/// Represents the response DTO for job application information.
/// </summary>
/// <remarks>
/// This DTO contains details about a worker's application to a job posting,
/// including the cover letter and offered cost.
/// </remarks>
public class JobApplicationResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the job application.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the job posting this application is for.
    /// </summary>
    public required string JobPostingId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the applicant (worker).
    /// </summary>
    public required string ApplicantId { get; set; }

    /// <summary>
    /// Gets or sets the cover letter submitted with the application.
    /// </summary>
    public string? CoverLetter { get; set; }

    /// <summary>
    /// Gets or sets the cost proposed by the applicant for completing the job.
    /// </summary>
    public required decimal OfferedCost { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the application was submitted.
    /// </summary>
    public DateTime AppliedAt { get; set; }

    /// <summary>
    /// Gets or sets the applicant's profile information.
    /// </summary>
    public ApplicantResponse? Applicant { get; set; }
}
