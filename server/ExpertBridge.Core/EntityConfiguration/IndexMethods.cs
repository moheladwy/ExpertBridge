namespace ExpertBridge.Core.EntityConfiguration;

/// <summary>
///     Defines PostgreSQL index method constants for Entity Framework Core configuration.
/// </summary>
/// <remarks>
///     Used in entity configurations to specify index types for optimal query performance.
///     GIN (Generalized Inverted Index) is used for full-text search and array operations.
///     HNSW (Hierarchical Navigable Small World) is used for vector similarity search with pgvector.
/// </remarks>
public static class IndexMethods
{
    /// <summary>
    ///     GIN (Generalized Inverted Index) index method for full-text search and array operations.
    /// </summary>
    public const string Gin = "GIN";

    /// <summary>
    ///     HNSW (Hierarchical Navigable Small World) index method for efficient vector similarity search using pgvector.
    /// </summary>
    public const string Hnsw = "hnsw";
}
