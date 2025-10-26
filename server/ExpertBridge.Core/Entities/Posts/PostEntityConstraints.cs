// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.Posts;

/// <summary>
///     Defines validation constraints for Post entity properties.
/// </summary>
/// <remarks>
///     These constraints are used in Entity Framework Core configurations and FluentValidation validators.
/// </remarks>
public class PostEntityConstraints
{
    /// <summary>
    ///     Maximum length for post title (256 characters).
    /// </summary>
    public const int MaxTitleLength = 256;

    /// <summary>
    ///     Maximum length for post content (5000 characters).
    /// </summary>
    public const int MaxContentLength = 5000;
}
