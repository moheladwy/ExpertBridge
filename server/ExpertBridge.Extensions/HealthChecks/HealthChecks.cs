using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace ExpertBridge.Extensions.HealthChecks;

/// <summary>
///     Provides extension methods for configuring health checks in an application.
/// </summary>
public static class HealthChecks
{
    /// <summary>
    ///     Adds default health checks to the application, including a self-check and a PostgreSQL database check.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder used to configure the application's health checks.</param>
    /// <returns>The updated host application builder with the health checks configured.</returns>
    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var connectionString = builder.Configuration.GetConnectionString("Postgresql")!;
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
            .AddNpgSql(connectionString, tags: ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
            { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
        // Only health checks tagged with the "live" tag must pass for the app to be considered alive
        app.MapHealthChecks("/alive", new HealthCheckOptions
            { Predicate = r => r.Tags.Contains("live") });

        return app;
    }
}
