using Pgvector;

namespace ExpertBridge.Api.VectorSearch;

public interface IEmbeddingService
{
    Task<Vector> GenerateEmbedding(string text);
}
