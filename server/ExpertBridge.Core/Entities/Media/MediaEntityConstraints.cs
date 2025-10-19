// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.Media;

/// <summary>
/// Defines validation constraints for Media entity properties.
/// </summary>
/// <remarks>
/// These constraints are used in Entity Framework Core configurations and FluentValidation validators.
/// </remarks>
public class MediaEntityConstraints
{
    /// <summary>
    /// Maximum length for media file name (256 characters).
    /// </summary>
    public const int MaxNameLength = 256;

    /// <summary>
    /// Maximum length for media URL (2048 characters).
    /// </summary>
    public const int MaxMediaUrlLength = 2048;
}
