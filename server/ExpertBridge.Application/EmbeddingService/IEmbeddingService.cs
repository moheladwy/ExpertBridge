// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Pgvector;

namespace ExpertBridge.Application.EmbeddingService;

/// <summary>
/// Defines the contract for embedding generation services that convert text into vector representations for semantic similarity operations.
/// </summary>
/// <remarks>
/// This interface abstracts the embedding generation process, allowing different implementations
/// (Ollama, OpenAI, Azure OpenAI, etc.) to be swapped without affecting dependent services.
/// 
/// **Purpose:**
/// Text embeddings are numerical vector representations of text that capture semantic meaning.
/// Similar texts produce similar vectors, enabling:
/// - Semantic search (find similar content by meaning, not just keywords)
/// - Content recommendations (suggest related posts/profiles)
/// - Job-expert matching (match skills to requirements)
/// - Duplicate content detection
/// - Clustering and classification
/// 
/// **Architecture Role:**
/// This interface is part of the domain layer and is implemented by infrastructure-specific services:
/// - OllamaEmbeddingService: Local embedding generation using Ollama
/// - (Future) OpenAIEmbeddingService: Cloud-based via OpenAI API
/// - (Future) AzureOpenAIEmbeddingService: Enterprise cloud via Azure
/// 
/// **Vector Storage:**
/// Generated vectors are stored in PostgreSQL using the pgvector extension, which provides:
/// - Efficient similarity search using cosine distance, L2 distance, or inner product
/// - Indexing for fast nearest-neighbor queries
/// - Native SQL integration for complex queries
/// 
/// **Use Cases in ExpertBridge:**
/// 
/// **1. User Profile Matching:**
/// - User skills/interests → Vector
/// - Job requirements → Vector
/// - Find best-matching experts using cosine similarity
/// 
/// **2. Content Discovery:**
/// - Post content → Vector
/// - User query → Vector
/// - Find semantically similar posts
/// 
/// **3. Profile Skills Embedding:**
/// - Combined skill tags → Vector
/// - Match with job postings
/// - Recommend learning paths
/// 
/// **4. User Interest Vectors:**
/// - User's interaction history → Aggregated vector
/// - Personalized content recommendations
/// 
/// **Vector Dimensions:**
/// The dimension of returned vectors depends on the embedding model:
/// - nomic-embed-text: 768 dimensions
/// - mxbai-embed-large: 1024 dimensions
/// - all-minilm: 384 dimensions
/// 
/// Database schema must match the model's output dimensions.
/// 
/// **Implementation Requirements:**
/// Implementations should:
/// - Handle text preprocessing (normalization, truncation)
/// - Support caching to avoid redundant API calls
/// - Implement retry logic for transient failures
/// - Log errors for monitoring
/// - Validate input text length against model limits
/// - Return normalized vectors for consistent similarity calculations
/// 
/// **Performance Considerations:**
/// - Embedding generation can be slow (100ms-5s depending on service)
/// - Cache frequently requested embeddings
/// - Process in background workers for non-urgent operations
/// - Batch multiple texts when possible
/// 
/// The interface uses Pgvector.Vector as the return type to ensure compatibility
/// with PostgreSQL storage and pgvector operations.
/// </remarks>
public interface IEmbeddingService
{
    /// <summary>
    /// Generates a vector embedding representation of the provided text for semantic similarity operations.
    /// </summary>
    /// <param name="text">
    /// The input text to convert into a vector embedding. Should be meaningful text (not empty or just whitespace).
    /// Length limits depend on the embedding model (typically 512-8192 tokens).
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a <see cref="Vector"/> representing
    /// the semantic meaning of the input text. The vector's dimensions match the embedding model's output size.
    /// </returns>
    /// <remarks>
    /// **Processing Flow:**
    /// 1. Validate and preprocess input text
    /// 2. Check cache for existing embedding (if caching is implemented)
    /// 3. Generate embedding via ML model (Ollama, OpenAI, etc.)
    /// 4. Convert model output to Pgvector.Vector format
    /// 5. Cache result for future requests
    /// 6. Return vector for storage or similarity calculation
    /// 
    /// **Vector Properties:**
    /// - Normalized: Unit length for cosine similarity
    /// - Fixed dimensions: Matches embedding model configuration
    /// - Immutable: Same text always produces same vector (deterministic)
    /// 
    /// **Similarity Calculation:**
    /// Once generated, vectors can be compared using:
    /// <code>
    /// // Cosine similarity (1.0 = identical, 0.0 = orthogonal, -1.0 = opposite)
    /// var similarity = 1 - vector1.CosineDistance(vector2);
    /// 
    /// // PostgreSQL query for similar content
    /// var similar = await dbContext.Posts
    ///     .OrderBy(p => p.ContentEmbedding.CosineDistance(queryVector))
    ///     .Take(10)
    ///     .ToListAsync();
    /// </code>
    /// 
    /// **Text Preprocessing:**
    /// Implementations should consider:
    /// - Removing excessive whitespace
    /// - Truncating to model's maximum length
    /// - Handling special characters
    /// - Normalizing language-specific characters
    /// 
    /// **Error Handling:**
    /// May throw:
    /// - ArgumentException: Empty or invalid text
    /// - InvalidOperationException: Embedding service unavailable
    /// - TimeoutException: Model inference timeout
    /// - HttpRequestException: API service failures
    /// 
    /// **Example Usage:**
    /// <code>
    /// // Generate embedding for post content
    /// var post = await dbContext.Posts.FindAsync(postId);
    /// var embedding = await embeddingService.GenerateEmbedding(post.Content);
    /// post.ContentEmbedding = embedding;
    /// await dbContext.SaveChangesAsync();
    /// 
    /// // Find similar posts
    /// var similarPosts = await dbContext.Posts
    ///     .Where(p => p.Id != postId)
    ///     .OrderBy(p => p.ContentEmbedding.CosineDistance(embedding))
    ///     .Take(5)
    ///     .ToListAsync();
    /// </code>
    /// 
    /// **Caching Strategy:**
    /// Implementations should cache embeddings because:
    /// - Same text often appears multiple times (common skills, tags)
    /// - Generation is computationally expensive
    /// - Improves response time for frequently accessed content
    /// - Reduces API costs for cloud-based services
    /// 
    /// **Best Practices:**
    /// - Validate text is not empty before calling
    /// - Handle model-specific token limits
    /// - Use in background workers for bulk operations
    /// - Monitor embedding quality and dimensions
    /// - Consider batching for multiple texts
    /// 
    /// The returned Vector is ready for immediate storage in PostgreSQL with pgvector extension.
    /// </remarks>
    Task<Vector> GenerateEmbedding(string text);
}
