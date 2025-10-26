// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExpertBridge.Extensions.Embeddings;

/// <summary>
///     Provides extension methods for configuring Ollama-based embedding generation services in the ExpertBridge
///     application.
///     Integrates Microsoft.Extensions.AI for semantic vector embedding generation used in AI-powered search and
///     recommendations.
/// </summary>
public static class Embedding
{
    /// <summary>
    ///     Registers the Ollama embedding generator as a singleton service for generating 1024-dimensional vector embeddings
    ///     from text.
    ///     Configures the embedding service with endpoint and model settings for semantic similarity operations.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder to configure embedding services for.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <remarks>
    ///     This method:
    ///     - Loads EmbeddingServiceSettings from the "Ollama" configuration section
    ///     - Registers settings as IOptions for dependency injection
    ///     - Creates an OllamaEmbeddingGenerator instance with configured endpoint and model
    ///     - Registers IEmbeddingGenerator&lt;string, Embedding&lt;float&gt;&gt; as singleton service
    ///     The embedding generator is used throughout the application for:
    ///     - **User Interest Embeddings**: Generating vector representations of user interests from tags for personalized
    ///     recommendations
    ///     - **Post Embeddings**: Creating semantic vectors of post content for similarity search
    ///     - **Job Posting Embeddings**: Generating embeddings for AI-powered job matching with candidate profiles
    ///     Embeddings are stored in PostgreSQL using pgvector extension and indexed with HNSW algorithm
    ///     for efficient cosine distance similarity queries.
    /// </remarks>
    public static TBuilder AddEmbeddingServices<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var config = builder.Configuration.GetSection(EmbeddingServiceSettings.Section);
        builder.Services.Configure<EmbeddingServiceSettings>(config);

        var settings = config.Get<EmbeddingServiceSettings>();

        builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(serviceProvider =>
            new OllamaEmbeddingGenerator(
                settings.Endpoint,
                settings.ModelId)
        );

        return builder;
    }
}
