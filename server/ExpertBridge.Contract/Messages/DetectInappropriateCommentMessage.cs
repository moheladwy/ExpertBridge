// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Contract.Messages;

/// <summary>
///     Represents a message to trigger content moderation for a comment via RabbitMQ/MassTransit.
/// </summary>
/// <remarks>
///     This message invokes AI moderation services to detect inappropriate content including
///     toxicity, threats, obscenity, and other harmful language categories in user comments.
/// </remarks>
public sealed class DetectInappropriateCommentMessage
{
    /// <summary>
    ///     Gets or sets the unique identifier of the comment to moderate.
    /// </summary>
    public string CommentId { get; set; }

    /// <summary>
    ///     Gets or sets the content of the comment.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the user who created the comment.
    /// </summary>
    public string AuthorId { get; set; }
}
