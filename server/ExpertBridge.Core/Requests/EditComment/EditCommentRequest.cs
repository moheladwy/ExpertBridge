// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.EditComment;

/// <summary>
///     Represents a request to edit an existing comment.
/// </summary>
/// <remarks>
///     After editing, comments are reprocessed by AI services for content moderation.
/// </remarks>
public class EditCommentRequest
{
    /// <summary>
    ///     Gets or sets the new text content for the comment.
    /// </summary>
    public string? Content { get; set; }
}
