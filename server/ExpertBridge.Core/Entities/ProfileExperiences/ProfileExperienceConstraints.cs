// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.ProfileExperiences;

/// <summary>
/// Defines validation constraints for ProfileExperience entity properties.
/// </summary>
/// <remarks>
/// These constraints are used in Entity Framework Core configurations and FluentValidation validators.
/// ProfileExperience represents a user's work experience entry on their professional profile.
/// </remarks>
public class ProfileExperienceConstraints
{
    /// <summary>
    /// Maximum length for job title (256 characters).
    /// </summary>
    public const int MaxTitleLength = 256;

    /// <summary>
    /// Maximum length for experience description (500 characters).
    /// </summary>
    public const int MaxDescriptionLength = 500;

    /// <summary>
    /// Maximum length for company name (500 characters).
    /// </summary>
    public const int MaxCompanyLength = 500;

    /// <summary>
    /// Maximum length for location (500 characters).
    /// </summary>
    public const int MaxLocationLength = 500;
}
