// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

/// <summary>
/// Represents the response DTO for job applicant information.
/// </summary>
/// <remarks>
/// This DTO extends author information with reputation metrics and job history
/// to help hirers evaluate applicants.
/// </remarks>
public class ApplicantResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the applicant's profile.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the applicant's user account.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Gets or sets the applicant's first name.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the applicant's last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the applicant's username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the applicant's job title.
    /// </summary>
    public string? JobTitle { get; set; }

    /// <summary>
    /// Gets or sets the applicant's reputation score.
    /// </summary>
    /// <remarks>
    /// Reputation is calculated from engagement metrics and job completion ratings.
    /// </remarks>
    public int Reputation { get; set; }

    /// <summary>
    /// Gets or sets the number of jobs the applicant has successfully completed.
    /// </summary>
    public int JobsDone { get; set; }

    /// <summary>
    /// Gets or sets the URL of the applicant's profile picture.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }
}
