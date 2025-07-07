using Microsoft.Extensions.AI;
using Pgvector;

namespace ExpertBridge.Api.EmbeddingService;

public class QueuedEmbeddingService : IEmbeddingService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly SemaphoreSlim _semaphore;
    private readonly ILogger<QueuedEmbeddingService> _logger;
    private const int MaxConcurrentRequests = 1;

    public QueuedEmbeddingService(
            IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
            ILogger<QueuedEmbeddingService> logger)
    {
        _embeddingGenerator = embeddingGenerator;
        _logger = logger;
        _semaphore = new SemaphoreSlim(MaxConcurrentRequests, MaxConcurrentRequests);
    }

    public async Task<Vector> GenerateEmbedding(string text)
    {
        try
        {
            _logger.LogInformation("Requesting embedding for text: {Text}", text);
            await _semaphore.WaitAsync();

            var generatedEmbeddings = await _embeddingGenerator.GenerateAsync([text]);
            var embedding = generatedEmbeddings.Single();

            _logger.LogInformation("Successfully generated embedding for text: {Text}", text);
            return new Vector(embedding.Vector);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding for text: {Text}", text);
            return new Vector(Array.Empty<float>());
        }
        finally
        {
            _semaphore.Release();
            _logger.LogInformation("Semaphore released after processing text: {Text}", text);
        }
    }
}
