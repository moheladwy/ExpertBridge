// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Messages;

/// <summary>
/// Represents a message to generate vector embeddings for a post via RabbitMQ/MassTransit.
/// </summary>
/// <remarks>
/// This message triggers embedding generation using Ollama to enable semantic search
/// and similarity-based recommendations using pgvector.
/// </remarks>
public class EmbedPostMessage
{
    /// <summary>
    /// Gets or sets the unique identifier of the post to embed.
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
    /// Gets or sets a value indicating whether this is a job posting.
    /// </summary>
    public required bool IsJobPosting { get; set; }
}
