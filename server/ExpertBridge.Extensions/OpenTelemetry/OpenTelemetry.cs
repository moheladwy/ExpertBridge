// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace ExpertBridge.Extensions.OpenTelemetry;

/// <summary>
/// Provides extension methods for configuring OpenTelemetry observability with metrics, tracing, and logging in the ExpertBridge application.
/// Integrates telemetry collection for distributed tracing, performance monitoring, and diagnostic logging.
/// </summary>
public static class OpenTelemetry
{
    /// <summary>
    /// Configures comprehensive OpenTelemetry instrumentation including service discovery, logging, metrics, and distributed tracing.
    /// Sets up telemetry collection with ASP.NET Core, HttpClient, and runtime instrumentation for observability.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder to configure OpenTelemetry for.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <remarks>
    /// This method configures three pillars of observability:
    ///
    /// **Logging:**
    /// - Integrates OpenTelemetry with .NET logging infrastructure
    /// - Includes formatted log messages in telemetry exports
    /// - Includes log scopes for contextual information
    ///
    /// **Metrics:**
    /// - ASP.NET Core instrumentation (request count, duration, status codes)
    /// - HttpClient instrumentation (outbound HTTP call metrics)
    /// - .NET Runtime instrumentation (GC, thread pool, exception counters)
    /// - Prometheus exporter for scraping metrics
    ///
    /// **Distributed Tracing:**
    /// - Trace sources for the application
    /// - ASP.NET Core request tracing (inbound HTTP requests)
    /// - HttpClient tracing (outbound HTTP calls to Firebase, Groq, Ollama, S3)
    /// - Enables end-to-end request flow visualization
    ///
    /// Also configures service discovery and optional OTLP exporter based on OTEL_EXPORTER_OTLP_ENDPOINT environment variable.
    /// </remarks>
    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddServiceDiscovery();

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
    /// Conditionally adds OTLP (OpenTelemetry Protocol) exporter for sending telemetry to external observability platforms.
    /// Enables exporting metrics, traces, and logs to backends like Jaeger, Zipkin, or Azure Application Insights.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder to configure OTLP exporter for.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <remarks>
    /// The OTLP exporter is only enabled if the "OTEL_EXPORTER_OTLP_ENDPOINT" environment variable or configuration value is set.
    /// This allows flexible deployment configurations:
    /// - Development: Disabled or local collector
    /// - Staging: Export to staging observability backend
    /// - Production: Export to production monitoring platform
    ///
    /// OTLP is the standardized protocol for OpenTelemetry data export, supporting gRPC and HTTP transports.
    /// </remarks>
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

    /// <summary>
    /// Configures default HttpClient settings for all typed and named HttpClient instances with resilience and service discovery.
    /// Applies standard retry policies and service discovery integration to all HTTP clients in the application.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder to configure HttpClient defaults for.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <remarks>
    /// This method applies two standard configurations to all HttpClient instances:
    ///
    /// **Standard Resilience Handler:**
    /// - Automatic retry with exponential backoff for transient HTTP failures (5xx errors, timeouts)
    /// - Circuit breaker pattern to prevent cascading failures
    /// - Request timeout policies for preventing hung requests
    ///
    /// **Service Discovery:**
    /// - Enables dynamic endpoint resolution for service-to-service communication
    /// - Supports container orchestration and cloud-native deployments
    /// - Allows service names to be resolved to actual endpoints at runtime
    ///
    /// Applied to all HttpClient instances including those for Firebase, Groq, Ollama, and S3 integrations.
    /// </remarks>
    public static TBuilder ConfigureHttpClientDefaults<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });

        return builder;
    }
}
