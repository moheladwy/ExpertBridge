// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.Areas;

/// <summary>
/// Defines validation constraints for Area entity properties.
/// </summary>
/// <remarks>
/// These constraints are used in Entity Framework Core configurations and FluentValidation validators.
/// Areas represent geographical locations for job postings and user profiles.
/// </remarks>
public class AreaEntityConstraints
{
    /// <summary>
    /// Maximum length for governorate name (256 characters).
    /// </summary>
    public const int MaxGovernorateLength = 256;

    /// <summary>
    /// Maximum length for region name (256 characters).
    /// </summary>
    public const int MaxRegionLength = 256;
}
