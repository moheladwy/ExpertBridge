// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace ExpertBridge.Extensions.HealthChecks;

/// <summary>
/// Provides extension methods for configuring application health checks and health monitoring endpoints.
/// Implements health checks for critical infrastructure including self-check, PostgreSQL database, and Redis cache.
/// </summary>
public static class HealthChecks
{
    /// <summary>
    /// Registers health checks for application liveness monitoring including self-check, PostgreSQL database connectivity, and Redis cache availability.
    /// All checks are tagged with "live" for use in Kubernetes liveness probes and container orchestration.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder to configure health checks for.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <remarks>
    /// This method registers three health checks:
    /// 
    /// **Self Check:**
    /// - Always returns healthy status
    /// - Verifies the application process is running and responsive
    /// - Tagged "live" for liveness probes
    /// 
    /// **PostgreSQL Check:**
    /// - Tests database connectivity using the configured connection string
    /// - Verifies database is accepting connections and queries
    /// - Critical for data persistence operations
    /// - Tagged "live" for liveness probes
    /// 
    /// **Redis Check:**
    /// - Tests Redis cache connectivity and availability
    /// - 30-second timeout to avoid hanging health checks
    /// - Critical for distributed caching and session management
    /// - Tagged "live" for liveness probes
    /// 
    /// Health check endpoints are exposed via MapDefaultEndpoints method for monitoring systems and orchestrators.
    /// </remarks>
    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var npgsqlConnectionString = builder.Configuration.GetConnectionString("Postgresql")!;
        var redisConnectionString = builder.Configuration.GetConnectionString("Redis")!;
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
            .AddNpgSql(npgsqlConnectionString, tags: ["live"])
            .AddRedis(redisConnectionString, "Redis", tags: ["live"], timeout: TimeSpan.FromSeconds(30));

        return builder;
    }

    /// <summary>
    /// Maps health check endpoints to the application including detailed health status and liveness probe endpoints.
    /// Exposes /health for comprehensive health information and /alive for simple liveness checks.
    /// </summary>
    /// <param name="app">The web application to map health check endpoints to.</param>
    /// <returns>The web application instance for method chaining.</returns>
    /// <remarks>
    /// This method configures two health check endpoints:
    /// 
    /// **/health Endpoint:**
    /// - Returns detailed health status of all registered health checks
    /// - Uses HealthChecks UI response writer for formatted JSON output
    /// - Includes individual check names, status, duration, and error details
    /// - Suitable for monitoring dashboards and detailed health inspection
    /// - Returns 200 OK if all checks pass, 503 Service Unavailable if any fail
    /// 
    /// **/alive Endpoint:**
    /// - Returns only checks tagged with "live" (self, PostgreSQL, Redis)
    /// - Lightweight endpoint for Kubernetes liveness probes
    /// - Fast response for container orchestration health monitoring
    /// - Returns 200 OK if live checks pass, 503 Service Unavailable otherwise
    /// 
    /// These endpoints are typically excluded from authentication requirements for monitoring system access.
    /// </remarks>
    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health",
            new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
        // Only health checks tagged with the "live" tag must pass for the app to be considered alive
        app.MapHealthChecks("/alive", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });

        return app;
    }
}
