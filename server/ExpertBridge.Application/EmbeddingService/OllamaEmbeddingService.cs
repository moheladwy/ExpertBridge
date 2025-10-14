using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Pgvector;

namespace ExpertBridge.Application.EmbeddingService;

public class OllamaEmbeddingService(
    ILogger<OllamaEmbeddingService> logger,
    HybridCache cache,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    : IEmbeddingService
{
    public async Task<Vector> GenerateEmbedding(string text)
    {
        try
        {
            var cacheKey = $"Embedding:{text}";

            var cachedEmbedding = await cache.GetOrCreateAsync<Vector>(
                cacheKey,
                async ct =>
                {
                    try
                    {
                        var generatedEmbeddings =
                            await embeddingGenerator.GenerateAsync([text], cancellationToken: ct);
                        var embedding = generatedEmbeddings.Single();

                        logger.LogInformation("Successfully generated embedding for text: {Text}", text);
                        return new Vector(embedding.Vector);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error generating embedding for text: {Text}", text);
                        throw;
                    }
                });
            logger.LogInformation("Returning cached embedding for text: {Text}", text);
            return cachedEmbedding;
        }
        catch (Exception ex)
        {
            // Consider logging the exception here if a logging framework is available.
            logger.LogError(ex, "Error generating embedding for text: {Text}", text);
            throw new InvalidOperationException("Failed to generate embedding.", ex);
        }
    }
}
