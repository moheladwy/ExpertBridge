namespace ExpertBridge.Application.Settings.Serilog;

/// <summary>
/// Configuration arguments for Serilog sink-specific settings.
/// </summary>
/// <remarks>
/// This class contains common configuration properties used by various Serilog sinks.
/// Not all sinks use all properties - each sink consumes only the relevant arguments.
/// 
/// **OutputTemplate Property:**
/// Used by Console, File, and other text-based sinks to customize log message format.
/// 
/// **Template Syntax:**
/// - {Timestamp:format}: Event timestamp with custom format
/// - {Level:u3}: Log level uppercase, 3 characters (INF, WRN, ERR)
/// - {Message:lj}: Log message with literal JSON rendering
/// - {Exception}: Exception details (stack trace)
/// - {NewLine}: Line break
/// - {Properties}: All additional properties
/// 
/// **Example Templates:**
/// <code>
/// // Compact format for development
/// "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"
/// 
/// // Detailed format with source context
/// "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message}{NewLine}{Exception}"
/// 
/// // JSON-like format
/// "{Timestamp:o} {Level:u3} {Message:lj}{NewLine}{Exception}"
/// </code>
/// 
/// **ServerUrl Property:**
/// Used by remote sinks (Seq, Elasticsearch, Syslog) to specify the endpoint.
/// Format varies by sink but typically HTTP/HTTPS URL for web-based log servers.
/// 
/// Additional sink arguments can be configured in appsettings.json even if not explicitly defined here,
/// as Serilog's configuration system uses reflection to match JSON properties to sink parameters.
/// </remarks>
public class Args
{
    /// <summary>
    /// Gets or sets the output template format string for text-based sinks (Console, File).
    /// </summary>
    /// <remarks>
    /// Optional. If null, the sink's default format is used.
    /// 
    /// Common placeholders:
    /// - {Timestamp:HH:mm:ss}: Time only
    /// - {Level:u3}: Abbreviated log level (INF, WRN, ERR)
    /// - {Message:lj}: Message with literal JSON
    /// - {Exception}: Exception stack trace
    /// - {SourceContext}: Logger category name
    /// - {ThreadId}: Thread identifier
    /// 
    /// See Serilog documentation for complete formatting options.
    /// </remarks>
    public string? OutputTemplate { get; set; }

    /// <summary>
    /// Gets or sets the server URL for remote log sinks (Seq, Elasticsearch, Graylog).
    /// </summary>
    /// <remarks>
    /// Optional. Required only for sinks that send logs to remote servers.
    /// 
    /// Examples:
    /// - Seq: "http://localhost:5341"
    /// - Elasticsearch: "http://elasticsearch:9200"
    /// - Graylog: "http://graylog:12201/gelf"
    /// 
    /// Should include protocol (http/https) and port if non-standard.
    /// </remarks>
    public string? ServerUrl { get; set; }
}
