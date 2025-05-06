using Pgvector;

namespace Api.EmbeddingService;

public interface IEmbeddingService
{
    Task<Vector> GenerateEmbedding(string text);
}
