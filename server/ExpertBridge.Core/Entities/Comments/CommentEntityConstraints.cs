// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.Comments;

/// <summary>
/// Defines validation constraints for Comment entity properties.
/// </summary>
/// <remarks>
/// These constraints are used in Entity Framework Core configurations and FluentValidation validators.
/// </remarks>
public class CommentEntityConstraints
{
    /// <summary>
    /// Maximum length for comment content (5000 characters).
    /// </summary>
    public const int MaxContentLength = 5000;
}
