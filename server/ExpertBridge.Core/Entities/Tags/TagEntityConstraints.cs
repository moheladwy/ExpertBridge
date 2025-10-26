// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.Tags;

/// <summary>
///     Defines validation constraints for Tag entity properties.
/// </summary>
/// <remarks>
///     These constraints are used in Entity Framework Core configurations and FluentValidation validators.
///     Tags support multilingual names (English and Arabic) for content categorization.
/// </remarks>
public class TagEntityConstraints
{
    /// <summary>
    ///     Maximum length for tag name in any language (256 characters).
    /// </summary>
    public const int MaxNameLength = 256;

    /// <summary>
    ///     Maximum length for tag description (512 characters).
    /// </summary>
    public const int MaxDescriptionLength = 512;
}
