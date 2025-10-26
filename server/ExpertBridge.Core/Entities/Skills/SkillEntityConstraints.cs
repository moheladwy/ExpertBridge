// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.Skills;

/// <summary>
///     Defines validation constraints for Skill entity properties.
/// </summary>
/// <remarks>
///     These constraints are used in Entity Framework Core configurations and FluentValidation validators.
/// </remarks>
public class SkillEntityConstraints
{
    /// <summary>
    ///     Maximum length for skill name (256 characters).
    /// </summary>
    public const int MaxNameLength = 256;

    /// <summary>
    ///     Maximum length for skill description (256 characters).
    /// </summary>
    public const int MaxDescriptionLength = 256;
}
