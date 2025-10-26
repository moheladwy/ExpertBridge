// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.JobPostings;

/// <summary>
/// Defines validation constraints for JobPosting entity properties.
/// </summary>
/// <remarks>
/// These constraints are used in Entity Framework Core configurations and FluentValidation validators.
/// </remarks>
public class JobPostingEntityConstraints
{
    /// <summary>
    /// Maximum length for job posting title (256 characters).
    /// </summary>
    public const int MaxTitleLength = 256;

    /// <summary>
    /// Maximum length for job posting content/description (5000 characters).
    /// </summary>
    public const int MaxContentLength = 5000;

    /// <summary>
    /// Minimum budget value for job postings (0).
    /// </summary>
    public const int MinBudget = 0;
}
