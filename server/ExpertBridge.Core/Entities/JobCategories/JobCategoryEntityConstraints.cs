// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.JobCategories;

/// <summary>
/// Defines validation constraints for JobCategory entity properties.
/// </summary>
/// <remarks>
/// These constraints are used in Entity Framework Core configurations and FluentValidation validators.
/// </remarks>
public class JobCategoryEntityConstraints
{
    /// <summary>
    /// Maximum length for job category name (256 characters).
    /// </summary>
    public const int MaxNameLength = 256;

    /// <summary>
    /// Maximum length for job category description (512 characters).
    /// </summary>
    public const int MaxDescriptionLength = 512;
}
