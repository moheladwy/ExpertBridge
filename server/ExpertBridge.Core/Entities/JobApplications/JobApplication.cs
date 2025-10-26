// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.JobApplications;

/// <summary>
/// Represents a job application submitted by a worker to a job posting.
/// </summary>
/// <remarks>
/// Job applications allow workers to express interest in job postings by submitting their proposed cost and cover letter.
/// The hirer can review applications and either accept one to create a job contract or decline them.
/// </remarks>
public class JobApplication : BaseModel, ISoftDeletable
{
    /// <summary>
    /// Gets or sets the unique identifier of the job posting being applied to.
    /// </summary>
    public required string JobPostingId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the applicant (worker).
    /// </summary>
    public required string ApplicantId { get; set; }

    /// <summary>
    /// Gets or sets the applicant's cover letter explaining their qualifications and approach.
    /// </summary>
    public string? CoverLetter { get; set; }

    /// <summary>
    /// Gets or sets the cost proposed by the applicant for completing the job.
    /// </summary>
    public required decimal OfferedCost { get; set; }

    /// <summary>
    /// Gets or sets the profile of the applicant.
    /// </summary>
    public Profile Applicant { get; set; }

    /// <summary>
    /// Gets or sets the job posting this application is for.
    /// </summary>
    public JobPosting JobPosting { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the application is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the application was marked as deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
