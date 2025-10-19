namespace ExpertBridge.Application.Settings.Serilog;

/// <summary>
/// Configuration for Serilog minimum log level filtering with namespace-specific overrides.
/// </summary>
/// <remarks>
/// Controls which log events are captured based on severity level (Verbose, Debug, Information, Warning, Error, Fatal).
/// Provides a default level and allows fine-grained control over specific namespaces.
/// 
/// **Example Configuration:**
/// <code>
/// {
///   "MinimumLevel": {
///     "Default": "Information",
///     "Override": {
///       "Microsoft": "Warning",
///       "Microsoft.EntityFrameworkCore": "Warning",
///       "System": "Warning",
///       "ExpertBridge.Application": "Debug"
///     }
///   }
/// }
/// </code>
/// 
/// **Log Levels (from least to most severe):**
/// - Verbose: Very detailed, typically only enabled in troubleshooting
/// - Debug: Diagnostic information useful during development
/// - Information: General informational messages (default for production)
/// - Warning: Potential issues or unusual situations
/// - Error: Errors and exceptions
/// - Fatal: Critical errors causing application shutdown
/// 
/// **Common Override Patterns:**
/// - Set Microsoft/System to Warning to reduce framework noise
/// - Set specific namespaces to Debug for detailed troubleshooting
/// - Set EntityFrameworkCore to Warning to avoid SQL query logging in production
/// 
/// Events below the configured level are discarded and not written to any sink.
/// </remarks>
public class MinimumLevel
{
    /// <summary>
    /// Gets or sets the default minimum log level applied to all loggers unless overridden.
    /// </summary>
    /// <remarks>
    /// Recommended values:
    /// - Development: "Debug" or "Information"
    /// - Production: "Information" or "Warning"
    /// 
    /// Valid values: "Verbose", "Debug", "Information", "Warning", "Error", "Fatal"
    /// </remarks>
    public string Default { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the dictionary of namespace-specific log level overrides.
    /// </summary>
    /// <remarks>
    /// Key: Fully qualified namespace (e.g., "Microsoft.EntityFrameworkCore")
    /// Value: Log level string (e.g., "Warning")
    /// 
    /// Overrides are matched hierarchically - "Microsoft" applies to all Microsoft.* namespaces.
    /// More specific overrides take precedence: "Microsoft.EntityFrameworkCore" beats "Microsoft".
    /// 
    /// Common overrides:
    /// - "Microsoft": "Warning" - Reduce ASP.NET Core framework logging
    /// - "System": "Warning" - Reduce System.* logging
    /// - "Microsoft.EntityFrameworkCore.Database.Command": "Warning" - Hide SQL queries
    /// </remarks>
    public Dictionary<string, string> Override { get; set; } = new();
}
