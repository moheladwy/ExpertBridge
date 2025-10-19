// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Application.Settings.Serilog;

/// <summary>
/// Configuration for a Serilog sink destination where log events are written.
/// </summary>
/// <remarks>
/// Each WriteTo entry represents one logging sink (output destination) with its configuration.
/// Serilog supports multiple sinks simultaneously, allowing logs to be written to console, file, databases, cloud services, etc.
/// 
/// **Example Configurations:**
/// 
/// **Console Sink:**
/// <code>
/// {
///   "Name": "Console",
///   "Args": {
///     "OutputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
///   }
/// }
/// </code>
/// 
/// **File Sink with Rolling:**
/// <code>
/// {
///   "Name": "File",
///   "Args": {
///     "path": "logs/app-.log",
///     "rollingInterval": "Day",
///     "retainedFileCountLimit": 7
///   }
/// }
/// </code>
/// 
/// **Seq Sink (Structured Log Server):**
/// <code>
/// {
///   "Name": "Seq",
///   "Args": {
///     "serverUrl": "http://localhost:5341"
///   }
/// }
/// </code>
/// 
/// **Common Sinks:**
/// - Console: Development debugging output
/// - File: Persistent logs with rolling policies
/// - Seq: Structured log server for querying and analysis
/// - ApplicationInsights: Azure cloud logging and telemetry
/// - Elasticsearch: Log aggregation and search
/// - Syslog: System logging protocols
/// 
/// The Args property contains sink-specific configuration matching the sink's constructor parameters.
/// </remarks>
public class WriteTo
{
    /// <summary>
    /// Gets or sets the name of the Serilog sink (e.g., "Console", "File", "Seq").
    /// </summary>
    /// <remarks>
    /// Must match a registered sink extension method name from the configured "Using" assemblies.
    /// For example, "Console" maps to WriteTo.Console() from Serilog.Sinks.Console package.
    /// </remarks>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sink-specific configuration arguments.
    /// </summary>
    /// <remarks>
    /// Properties in Args map to the sink's configuration parameters.
    /// Common properties:
    /// - OutputTemplate: Custom log message format
    /// - ServerUrl: Remote server endpoint (Seq, Elasticsearch, etc.)
    /// - Path: File path for file-based sinks
    /// - RollingInterval: File rolling policy (Day, Hour, etc.)
    /// 
    /// See specific sink documentation for available arguments.
    /// </remarks>
    public Args Args { get; set; } = new();
}
