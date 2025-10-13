using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Pgvector;

namespace ExpertBridge.Application.EmbeddingService;

public sealed class QueuedEmbeddingService : IEmbeddingService
{
    private const int MaxConcurrentRequests = 1;
    private readonly HybridCache _cache;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly ILogger<QueuedEmbeddingService> _logger;
    private readonly SemaphoreSlim _semaphore;

    public QueuedEmbeddingService(
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        ILogger<QueuedEmbeddingService> logger,
        HybridCache cache)
    {
        _embeddingGenerator = embeddingGenerator;
        _logger = logger;
        _cache = cache;
        _semaphore = new SemaphoreSlim(MaxConcurrentRequests, MaxConcurrentRequests);
    }

    public async Task<Vector> GenerateEmbedding(string text)
    {
        try
        {
            _logger.LogInformation("Requesting embedding for text: {Text}", text);
            var cacheKey = $"Embedding:{text}";

            var cachedEmbedding = await _cache.GetOrCreateAsync<Vector>(cacheKey,
                async ct =>
                {
                    try
                    {
                        await _semaphore.WaitAsync();

                        var generatedEmbeddings =
                            await _embeddingGenerator.GenerateAsync([text], cancellationToken: ct);
                        var embedding = generatedEmbeddings.Single();

                        _logger.LogInformation("Successfully generated embedding for text: {Text}", text);
                        return new Vector(embedding.Vector);
                    }
                    finally
                    {
                        _semaphore.Release();
                        _logger.LogInformation("Semaphore released after generating embedding for text: {Text}", text);
                    }
                });

            _logger.LogInformation("Returning cached embedding for text: {Text}", text);
            return cachedEmbedding;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding for text: {Text}", text);
            return new Vector(Array.Empty<float>());
        }
    }
}
