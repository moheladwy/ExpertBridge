// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace ExpertBridge.Api.Extensions;

/// <summary>
///     Provides extension methods for configuring OpenTelemetry.
/// </summary>
internal static class OpenTelemetry
{
    /// <summary>
    ///     Configures OpenTelemetry for the application.
    /// </summary>
    /// <typeparam name="TBuilder">
    ///     The type of the application builder.
    /// </typeparam>
    /// <param name="builder">
    ///     The application builder to configure OpenTelemetry for.
    /// </param>
    /// <returns>
    ///     The application builder with OpenTelemetry configured.
    /// </returns>
    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter();
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    /// <summary>
    ///     Adds the OpenTelemetry exporters to the application builder.
    /// </summary>
    /// <param name="builder">
    ///     The WebApplicationBuilder to add the OpenTelemetry exporters to.
    /// </param>
    /// <typeparam name="TBuilder">
    ///     The type of the WebApplicationBuilder.
    /// </typeparam>
    /// <returns>
    ///     The WebApplicationBuilder with the OpenTelemetry exporters added.
    /// </returns>
    private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        return builder;
    }
}
