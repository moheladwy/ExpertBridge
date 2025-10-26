// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.Badges;

/// <summary>
/// Defines validation constraints for Badge entity properties.
/// </summary>
/// <remarks>
/// These constraints are used in Entity Framework Core configurations and FluentValidation validators.
/// </remarks>
public class BadgeEntityConstraints
{
    /// <summary>
    /// Maximum length for badge name (256 characters).
    /// </summary>
    public const int MaxNameLength = 256;

    /// <summary>
    /// Maximum length for badge description (500 characters).
    /// </summary>
    public const int MaxDescriptionLength = 500;
}
