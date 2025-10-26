// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Pgvector;

namespace ExpertBridge.Core.Interfaces;

/// <summary>
///     Defines the contract for content that can be recommended using AI-powered similarity search.
/// </summary>
/// <remarks>
///     Implementing this interface enables content to be embedded using vector embeddings and searched
///     for similarity using PostgreSQL pgvector extension for semantic recommendations.
/// </remarks>
public interface IRecommendableContent
{
    /// <summary>
    ///     Gets or sets the unique identifier for the content.
    /// </summary>
    string Id { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the content author.
    /// </summary>
    string AuthorId { get; set; }

    /// <summary>
    ///     Gets or sets the title of the content.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    ///     Gets or sets the main body or description of the content.
    /// </summary>
    string Content { get; set; }

    /// <summary>
    ///     Gets or sets the detected language of the content.
    /// </summary>
    /// <remarks>
    ///     This property is populated by AI language detection services during content processing.
    /// </remarks>
    string? Language { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the content has been processed through the AI pipeline.
    /// </summary>
    bool IsProcessed { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the content has been automatically tagged.
    /// </summary>
    bool IsTagged { get; set; }

    /// <summary>
    ///     Gets or sets the vector embedding representation of the content for similarity search.
    /// </summary>
    /// <remarks>
    ///     The embedding is generated using AI models (e.g., Ollama) and stored using PostgreSQL pgvector extension.
    ///     This enables semantic similarity search and content recommendations.
    /// </remarks>
    Vector? Embedding { get; set; }
}
