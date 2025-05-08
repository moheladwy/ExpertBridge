namespace ExpertBridge.Extensions.Embeddings;

public sealed class EmbeddingServiceSettings
{
    public const string Section = "Ollama";

    public string Endpoint { get; set; }
    public string ModelId { get; set; }
}
