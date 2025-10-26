// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.JobReviews;

/// <summary>
///     Defines validation constraints for JobReview entity properties.
/// </summary>
/// <remarks>
///     These constraints are used in Entity Framework Core configurations and FluentValidation validators.
///     JobReviews allow users to rate and review completed jobs.
/// </remarks>
public static class JobReviewEntityConstraints
{
    /// <summary>
    ///     Maximum length for review text (5000 characters).
    /// </summary>
    public const int MaxReviewLength = 5000;

    /// <summary>
    ///     Maximum rating value (5 stars).
    /// </summary>
    public const int MaxRating = 5;

    /// <summary>
    ///     Minimum rating value (0 stars).
    /// </summary>
    public const int MinRating = 0;
}
