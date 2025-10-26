namespace ExpertBridge.Application.Settings.Serilog;

/// <summary>
///     Configuration settings for Serilog structured logging framework.
/// </summary>
/// <remarks>
///     This settings class mirrors the Serilog configuration format from appsettings.json,
///     enabling strongly-typed access to logging configuration.
///     **Configured in appsettings.json under "Serilog" section:**
///     <code>
/// {
///   "Serilog": {
///     "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
///     "MinimumLevel": {
///       "Default": "Information",
///       "Override": {
///         "Microsoft": "Warning",
///         "System": "Warning"
///       }
///     },
///     "WriteTo": [
///       {
///         "Name": "Console",
///         "Args": {
///           "OutputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
///         }
///       },
///       {
///         "Name": "File",
///         "Args": {
///           "path": "logs/app-.log",
///           "rollingInterval": "Day"
///         }
///       }
///     ],
///     "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"]
///   }
/// }
/// </code>
///     **Serilog Components:**
///     - Using: NuGet packages/sinks to load at runtime
///     - MinimumLevel: Log level filtering (Verbose, Debug, Information, Warning, Error, Fatal)
///     - WriteTo: Output destinations (Console, File, Seq, Application Insights, etc.)
///     - Enrich: Add contextual properties (machine name, thread ID, correlation ID, etc.)
///     **Typical Sinks:**
///     - Console: Development debugging
///     - File: Persistent local logs with rolling
///     - Seq: Structured log server for querying and analysis
///     - Application Insights: Azure cloud logging
///     Serilog is initialized early in Program.cs before the host builder for capturing startup errors.
/// </remarks>
public class SerilogSettings
{
    /// <summary>
    ///     The configuration section name in appsettings.json.
    /// </summary>
    public const string Section = "Serilog";

    /// <summary>
    ///     Gets or sets the array of Serilog sink assemblies to load (e.g., "Serilog.Sinks.Console").
    /// </summary>
    /// <remarks>
    ///     These must match NuGet package names and be installed in the project.
    ///     Serilog dynamically loads these assemblies at runtime for extensibility.
    /// </remarks>
    public string[] Using { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     Gets or sets the minimum log level configuration including default and namespace-specific overrides.
    /// </summary>
    /// <remarks>
    ///     Controls which log events are captured based on severity.
    ///     Common pattern: Default = Information, Override Microsoft/System to Warning to reduce noise.
    /// </remarks>
    public MinimumLevel MinimumLevel { get; set; } = new();

    /// <summary>
    ///     Gets or sets the array of sink configurations defining where logs are written.
    /// </summary>
    /// <remarks>
    ///     Each sink has a Name (matching the sink type) and Args (sink-specific configuration).
    ///     Multiple sinks can be configured to write logs to different destinations simultaneously.
    /// </remarks>
    public WriteTo[] WriteTo { get; set; } = Array.Empty<WriteTo>();

    /// <summary>
    ///     Gets or sets the array of enrichers that add contextual properties to log events.
    /// </summary>
    /// <remarks>
    ///     Common enrichers:
    ///     - FromLogContext: Adds properties from LogContext.PushProperty
    ///     - WithMachineName: Adds the machine/server name
    ///     - WithThreadId: Adds the thread identifier
    ///     - WithEnvironmentName: Adds the ASP.NET Core environment (Development, Production)
    ///     Enriched properties are available for filtering and searching in log aggregation tools.
    /// </remarks>
    public string[] Enrich { get; set; } = Array.Empty<string>();
}
