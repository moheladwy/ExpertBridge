// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities;

/// <summary>
/// Defines global constraint constants used across multiple entity types.
/// </summary>
/// <remarks>
/// These constraints are used in Entity Framework Core configurations and FluentValidation validators
/// to ensure consistent data validation and database schema constraints throughout the application.
/// </remarks>
public static class GlobalEntitiesConstraints
{
    /// <summary>
    /// Maximum length for entity identifier fields (450 characters).
    /// </summary>
    public const int MaxIdLength = 450;

    /// <summary>
    /// Maximum length for enum string representations (128 characters).
    /// </summary>
    public const int MaxEnumsLength = 128;

    /// <summary>
    /// Maximum length for title fields (500 characters).
    /// </summary>
    public const int MaxTitleLength = 500;

    /// <summary>
    /// Maximum length for description fields (1000 characters).
    /// </summary>
    public const int MaxDescriptionLength = 1000;

    /// <summary>
    /// Maximum length for cover letter content in job applications (1000 characters).
    /// </summary>
    public const int MaxCoverLetterLength = 1000;

    /// <summary>
    /// Maximum length for general content fields (1000 characters).
    /// </summary>
    public const int MaxContentLetterLength = 1000;
}
