namespace ExpertBridge.Application.Settings;

/// <summary>
///     Configuration settings for the Ollama embedding service used for vector similarity search.
/// </summary>
/// <remarks>
///     This settings class configures the connection to Ollama, a local LLM inference server,
///     for generating text embeddings used in AI-powered features like semantic search and content recommendations.
///     **Configured in appsettings.json under "Ollama" section:**
///     <code>
/// {
///   "Ollama": {
///     "Endpoint": "http://localhost:11434",
///     "ModelId": "nomic-embed-text"
///   }
/// }
/// </code>
///     **Ollama Integration:**
///     - Ollama runs locally and provides embedding generation without cloud API costs
///     - Supports multiple embedding models (nomic-embed-text, mxbai-embed-large, etc.)
///     - Embeddings are stored in PostgreSQL using pgvector extension
///     - Used for semantic similarity search in profiles, posts, and job matching
///     **Embedding Use Cases:**
///     - User profile interest embeddings for personalized recommendations
///     - Post content embeddings for semantic search
///     - Job posting embeddings for skill-based matching
///     - Similar content discovery
///     **Model Selection:**
///     - nomic-embed-text: Good balance of speed and quality (768 dimensions)
///     - mxbai-embed-large: Higher quality, slower (1024 dimensions)
///     - all-minilm: Lightweight, faster (384 dimensions)
///     Ensure Ollama is running and the specified model is pulled before starting the application.
/// </remarks>
public sealed class EmbeddingServiceSettings
{
    /// <summary>
    ///     The configuration section name in appsettings.json.
    /// </summary>
    public const string Section = "Ollama";

    /// <summary>
    ///     Gets or sets the base URL endpoint for the Ollama service.
    /// </summary>
    /// <remarks>
    ///     Default local endpoint is http://localhost:11434.
    ///     Can be configured to point to a remote Ollama instance for production.
    /// </remarks>
    public string Endpoint { get; set; }

    /// <summary>
    ///     Gets or sets the embedding model identifier to use for generating vectors.
    /// </summary>
    /// <remarks>
    ///     Recommended models:
    ///     - "nomic-embed-text": Best for general text embedding (default)
    ///     - "mxbai-embed-large": Higher quality for critical applications
    ///     - "all-minilm": Fastest for development/testing
    ///     Model must be pulled in Ollama before use: `ollama pull nomic-embed-text`
    /// </remarks>
    public string ModelId { get; set; }
}
