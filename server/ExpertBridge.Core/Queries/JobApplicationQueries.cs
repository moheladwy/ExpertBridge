// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobApplications;
using ExpertBridge.Core.Responses;

namespace ExpertBridge.Core.Queries;

/// <summary>
/// Provides extension methods for querying and projecting JobApplication entities.
/// </summary>
/// <remarks>
/// These query extensions project job applications to response DTOs with applicant profile information.
/// </remarks>
public static class JobApplicationQueries
{
    /// <summary>
    /// Projects a queryable of JobApplication entities to JobApplicationResponse DTOs with applicant profiles.
    /// </summary>
    /// <param name="query">The source queryable of job applications.</param>
    /// <returns>A queryable of JobApplicationResponse objects with applicant details and reputation.</returns>
    public static IQueryable<JobApplicationResponse> SelectJobApplicationResponseFromEntity(
        this IQueryable<JobApplication> query)
    {
        return query
            .Select(j => SelectJobApplicationResponseFromEntity(j));
    }

    /// <summary>
    /// Projects a single JobApplication entity to a JobApplicationResponse DTO with applicant profile.
    /// </summary>
    /// <param name="j">The job application entity to project.</param>
    /// <returns>A JobApplicationResponse object with applicant details, cover letter, and offered cost.</returns>
    public static JobApplicationResponse SelectJobApplicationResponseFromEntity(this JobApplication j)
    {
        return new JobApplicationResponse
        {
            Id = j.Id,
            Applicant = j.Applicant.SelectApplicantResponseFromProfile(),
            ApplicantId = j.ApplicantId,
            JobPostingId = j.JobPostingId,
            OfferedCost = j.OfferedCost,
            AppliedAt = j.CreatedAt!.Value,
            CoverLetter = j.CoverLetter
        };
    }
}
