// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Interfaces;

/// <summary>
/// Defines the contract for content that can be moderated for safety and appropriateness.
/// </summary>
/// <remarks>
/// Implementing this interface enables automatic content moderation and flagging of inappropriate content.
/// </remarks>
public interface ISafeContent
{
    /// <summary>
    /// Gets or sets a value indicating whether the content has been verified as safe and appropriate.
    /// </summary>
    /// <remarks>
    /// Content is processed through AI-powered moderation services to detect inappropriate language or harmful content.
    /// </remarks>
    public bool IsSafeContent { get; set; }
}
