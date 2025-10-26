// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Extensions.Embeddings;

/// <summary>
/// Represents configuration settings for Ollama embedding generation service in the ExpertBridge application.
/// Defines the endpoint URL and model identifier for generating 1024-dimensional vector embeddings used in semantic search.
/// </summary>
/// <remarks>
/// These settings are loaded from the "Ollama" configuration section and configure the embedding service for:
/// - User interest embeddings (for personalized content recommendations)
/// - Post content embeddings (for semantic post search)
/// - Job posting embeddings (for AI-powered job matching)
///
/// The embeddings are stored in PostgreSQL with pgvector extension and indexed using HNSW algorithm
/// for efficient similarity search using cosine distance.
/// </remarks>
public sealed class EmbeddingServiceSettings
{
    /// <summary>
    /// Gets the configuration section name for Ollama embedding service settings.
    /// </summary>
    public const string Section = "Ollama";

    /// <summary>
    /// Gets or sets the base URL endpoint of the Ollama API service (e.g., "http://localhost:11434").
    /// </summary>
    public string Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the Ollama model identifier used for embedding generation (e.g., "nomic-embed-text", "mxbai-embed-large").
    /// The model determines the embedding dimension and quality of semantic representations.
    /// </summary>
    public string ModelId { get; set; }
}
