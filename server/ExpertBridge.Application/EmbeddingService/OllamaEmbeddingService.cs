using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Pgvector;

namespace ExpertBridge.Application.EmbeddingService;

/// <summary>
/// Provides text embedding generation using Ollama's local ML models.
/// </summary>
/// <remarks>
/// This service implements <see cref="IEmbeddingService"/> using Ollama, an open-source tool for running
/// large language models locally. It provides cost-effective, privacy-preserving embedding generation
/// without relying on cloud APIs.
/// </remarks>
public class OllamaEmbeddingService(
    ILogger<OllamaEmbeddingService> logger,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    : IEmbeddingService
{
    /// <summary>
    /// Generates a vector embedding for the provided text using Ollama.
    /// </summary>
    /// <param name="text">
    /// The input text to embed. Should be meaningful content (not empty or whitespace).
    /// Typical length: 10-512 tokens (longer text may be truncated by the model).
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a <see cref="Vector"/> with
    /// dimensions matching the configured Ollama embedding model (e.g., 768 for nomic-embed-text).
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="text"/> is null.
    /// </exception>
    public async Task<Vector> GenerateEmbedding(string text)
    {
        ArgumentNullException.ThrowIfNull(text, nameof(text));
        logger.LogInformation("Generating embedding for text: {Text}", text);

        var generatedEmbeddings = await embeddingGenerator.GenerateAsync([text]);
        var embedding = generatedEmbeddings.Single();

        logger.LogInformation("Successfully generated embedding for text: {Text}", text);
        return new Vector(embedding.Vector);
    }
}
