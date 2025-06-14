// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ExpertBridge.Extensions.Logging;

/// <summary>
///     Provides extension methods to configure Serilog logging in an ASP.NET Core application.
/// </summary>
public static class Serilog
{
    /// <summary>
    ///     Configures Serilog as the logging provider for the application using settings from configuration.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder" /> used to configure the ASP.NET Core application.</param>
    public static void AddSerilogLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(context.Configuration));

        builder.AddSeqEndpoint("Seq");
    }
}
