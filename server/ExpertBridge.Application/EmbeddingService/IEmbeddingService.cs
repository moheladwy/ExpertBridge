// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Pgvector;

namespace ExpertBridge.Application.EmbeddingService;

/// <summary>
///     Defines the contract for embedding generation services that convert text into vector representations for semantic
///     similarity operations.
/// </summary>
/// <remarks>
///     This interface abstracts the embedding generation process, allowing different implementations
///     (Ollama, OpenAI, Azure OpenAI, etc.) to be swapped without affecting dependent services.
///     **Purpose: **
///     Text embeddings are numerical vector representations of text that capture semantic meaning.
///     Similar texts produce similar vectors, enabling:
///     - Semantic search (find similar content by meaning, not just keywords)
///     - Content recommendations (suggest related posts/profiles)
///     - Job-expert matching (match skills to requirements)
///     - Duplicate content detection
///     - Clustering and classification
/// </remarks>
public interface IEmbeddingService
{
    /// <summary>
    ///     Generates a vector embedding representation of the provided text for semantic similarity operations.
    /// </summary>
    /// <param name="text">
    ///     The input text to convert into a vector embedding. Should be meaningful text (not empty or just whitespace).
    ///     Length limits depend on the embedding model (typically 512-8192 tokens).
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing a <see cref="Vector" /> representing
    ///     the semantic meaning of the input text. The vector's dimensions match the embedding model's output size.
    /// </returns>
    Task<Vector> GenerateEmbedding(string text);
}
