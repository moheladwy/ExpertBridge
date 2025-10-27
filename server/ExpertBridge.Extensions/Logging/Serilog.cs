// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Hosting;
using Serilog;

namespace ExpertBridge.Extensions.Logging;

/// <summary>
///     Provides extension methods for configuring Serilog structured logging in ASP.NET Core applications.
///     Replaces the default logging provider with Serilog for advanced logging features and centralized log aggregation.
/// </summary>
public static class Serilog
{
    /// <summary>
    ///     Configures Serilog as the primary logging provider for the application with configuration-based setup.
    ///     Replaces the default ASP.NET Core logging with Serilog's structured logging capabilities.
    /// </summary>
    /// <param name="builder">The web application builder to configure Serilog logging for.</param>
    /// <remarks>
    ///     This method configures Serilog with:
    ///     **Configuration-Based Setup:**
    ///     - Reads Serilog configuration from appsettings.json and appsettings.{Environment}.json
    ///     - Supports log level configuration, enrichers, and sink settings via configuration
    ///     - Allows dynamic log level changes without code recompilation
    ///     **Seq Integration:**
    ///     - Connects to Seq endpoint for centralized log aggregation and visualization
    ///     - Enables structured log querying and filtering in Seq dashboard
    ///     - Provides real-time log streaming and alerting capabilities
    ///     **Structured Logging Benefits:**
    ///     - Preserves log data as structured properties (not just strings)
    ///     - Enables powerful querying and filtering in log aggregation tools
    ///     - Supports log enrichment with contextual information (user, request ID, etc.)
    ///     - Provides better performance and searchability than text-based logging
    ///     The Seq endpoint is added via AddSeqEndpoint("Seq") for service discovery integration.
    /// </remarks>
    public static TBuilder AddSerilogLogging<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddSerilog(config =>
        {
            config.ReadFrom.Configuration(builder.Configuration);
        });

        return builder;
    }
}
