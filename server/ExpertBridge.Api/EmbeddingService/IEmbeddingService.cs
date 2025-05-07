using Pgvector;

namespace ExpertBridge.Api.EmbeddingService;

public interface IEmbeddingService
{
    Task<Vector> GenerateEmbedding(string text);
}
