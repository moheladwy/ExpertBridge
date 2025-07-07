using Microsoft.Extensions.AI;
using Pgvector;

namespace ExpertBridge.Api.EmbeddingService;

// DEPRECATED: Use QueuedEmbeddingService instead.
public class OllamaEmbeddingService(
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator
    ) : IEmbeddingService
{
    public async Task<Vector> GenerateEmbedding(string text)
    {
        var generatedEmbeddings = await embeddingGenerator.GenerateAsync([text]);
        var embedding = generatedEmbeddings.Single();
        return new Vector(embedding.Vector);
    }
}
