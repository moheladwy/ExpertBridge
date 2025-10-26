// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.Profiles;

/// <summary>
///     Defines validation constraints for Profile entity properties.
/// </summary>
/// <remarks>
///     These constraints are used in Entity Framework Core configurations and FluentValidation validators.
/// </remarks>
public static class ProfileEntityConstraints
{
    /// <summary>
    ///     Maximum length for job title (256 characters).
    /// </summary>
    public const int JobTitleMaxLength = 256;

    /// <summary>
    ///     Maximum length for bio/description (5000 characters).
    /// </summary>
    public const int BioMaxLength = 5000;

    /// <summary>
    ///     Minimum rating value (0 stars).
    /// </summary>
    public const int RatingMinValue = 0;

    /// <summary>
    ///     Maximum rating value (5 stars).
    /// </summary>
    public const int RatingMaxValue = 5;

    /// <summary>
    ///     Minimum number of ratings received (0).
    /// </summary>
    public const int RatingCountMinValue = 0;

    /// <summary>
    ///     Maximum number of ratings received (int.MaxValue).
    /// </summary>
    public const int RatingCountMaxValue = int.MaxValue;
}
