// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Messages;

/// <summary>
/// Represents a message to trigger content moderation for a post via RabbitMQ/MassTransit.
/// </summary>
/// <remarks>
/// This message invokes AI moderation services to detect inappropriate content including
/// toxicity, threats, obscenity, and other harmful language categories.
/// </remarks>
public class DetectInappropriatePostMessage
{
    /// <summary>
    /// Gets or sets the unique identifier of the post to moderate.
    /// </summary>
    public string PostId { get; set; }

    /// <summary>
    /// Gets or sets the title of the post.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the content of the post.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who created the post.
    /// </summary>
    public string AuthorId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a job posting.
    /// </summary>
    public required bool IsJobPosting { get; set; }
}
