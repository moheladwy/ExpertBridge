namespace ExpertBridge.Application.Settings;

/// <summary>
/// Configuration settings for database and cache connection strings.
/// </summary>
/// <remarks>
/// This settings class contains connection strings for the application's data persistence and caching infrastructure.
/// 
/// **Configured in appsettings.json under "ConnectionStrings" section:**
/// <code>
/// {
///   "ConnectionStrings": {
///     "Postgresql": "Host=localhost;Database=expertbridge;Username=admin;Password=***",
///     "Redis": "localhost:6379,password=***,ssl=false"
///   }
/// }
/// </code>
/// 
/// **Security Considerations:**
/// - Connection strings should never be committed to source control with real credentials
/// - Use User Secrets for development: `dotnet user-secrets set "ConnectionStrings:Postgresql" "Host=..."`
/// - Use environment variables or Azure Key Vault for production deployments
/// - Sensitive data like passwords should be encrypted or stored securely
/// 
/// **PostgreSQL Connection String Format:**
/// - Host, Database, Username, Password are required
/// - Optional parameters: Port, Timeout, Pooling, SSL Mode
/// 
/// **Redis Connection String Format:**
/// - Format: "host:port,password=xxx,ssl=true/false"
/// - Supports multiple endpoints for clustering
/// - SSL recommended for production environments
/// </remarks>
public sealed class ConnectionStrings
{
    /// <summary>
    /// The configuration section name in appsettings.json.
    /// </summary>
    public const string Section = "ConnectionStrings";

    /// <summary>
    /// Gets or sets the PostgreSQL database connection string for Entity Framework Core.
    /// </summary>
    /// <remarks>
    /// Used by ExpertBridgeDbContext for all database operations including:
    /// - User and profile management
    /// - Posts, comments, and votes
    /// - Job postings and applications
    /// - Notifications and messages
    /// - Vector embeddings (pgvector extension)
    /// </remarks>
    public string Postgresql { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Redis cache connection string for distributed caching and session storage.
    /// </summary>
    /// <remarks>
    /// Used for:
    /// - Response caching (HybridCache)
    /// - Session state management
    /// - Distributed locks
    /// - Real-time SignalR backplane (optional)
    /// </remarks>
    public string Redis { get; set; } = string.Empty;
}
