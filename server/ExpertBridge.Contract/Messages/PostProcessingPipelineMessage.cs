// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Contract.Messages;

/// <summary>
///     Represents a message to initiate the post processing pipeline via RabbitMQ/MassTransit.
/// </summary>
/// <remarks>
///     This message triggers the complete AI processing workflow for posts and job postings,
///     including tagging, embedding generation, and content moderation if needed.
/// </remarks>
public sealed class PostProcessingPipelineMessage
{
    /// <summary>
    ///     Gets or sets the unique identifier of the post to process.
    /// </summary>
    public string PostId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the user who created the post.
    /// </summary>
    public string AuthorId { get; set; }

    /// <summary>
    ///     Gets or sets the title of the post.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     Gets or sets the content of the post.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this is a job posting.
    /// </summary>
    /// <remarks>
    ///     Job postings may have different processing rules or additional fields.
    /// </remarks>
    public required bool IsJobPosting { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the content passed moderation checks.
    /// </summary>
    /// <remarks>
    ///     If false, additional moderation steps may be required.
    /// </remarks>
    public required bool IsSafeContent { get; set; }
}
