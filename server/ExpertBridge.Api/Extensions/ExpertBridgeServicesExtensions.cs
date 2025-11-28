using ExpertBridge.Application.Settings;
using ExpertBridge.Data;
using ExpertBridge.Extensions.AWS;
using ExpertBridge.Extensions.Caching;
using ExpertBridge.Extensions.Embeddings;
using ExpertBridge.Extensions.OpenTelemetry;
using ExpertBridge.Extensions.Resilience;

namespace ExpertBridge.Api.Extensions;

/// <summary>
///     Provides extension methods for configuring and adding services required by the ExpertBridge application.
/// </summary>
internal static class ExpertBridgeServicesExtensions
{
    /// <summary>
    ///     Adds the necessary services for the ExpertBridge application.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance to configure.</param>
    /// <returns>The modified WebApplicationBuilder with the registered services.</returns>
    public static WebApplicationBuilder AddExpertBridgeServices(this WebApplicationBuilder builder)
    {
        builder.ConfigureOpenTelemetry();

        // CONSIDER!
        builder.ConfigureExpertBridgeSettings(); // Move all .Configure settings calls here in this method call?

        // Infrastructure
        builder.Services.AddDatabase(builder.Configuration);
        // builder.AddRedisDistributedCache(connectionName: "Redis");
        builder.AddFusionCache();
        builder.AddS3ObjectService();
        builder.AddRateLimiting();

        // Firebase
        builder.AddFirebaseApp();
        builder.AddFirebaseAuthentication();
        builder.AddHttpClientForFirebaseService();

        // External remote services
        builder.AddEmbeddingServices();

        builder.Services.AddDomainServices();
        builder.AddResiliencePipeline();

        var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        return builder;
    }

    /// <summary>
    ///     Configures the settings required by the ExpertBridge application.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance to configure.</param>
    /// <returns>The modified WebApplicationBuilder with the configured application settings.</returns>
    private static WebApplicationBuilder ConfigureExpertBridgeSettings(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<FirebaseSettings>(
            builder.Configuration.GetSection(FirebaseSettings.Section));

        builder.Services.Configure<FirebaseAuthSettings>(
            builder.Configuration.GetSection(FirebaseAuthSettings.Section));

        builder.Services.Configure<AwsSettings>(
            builder.Configuration.GetSection(AwsSettings.Section));

        builder.Services.Configure<RateLimitOptions>(
            builder.Configuration.GetSection(RateLimitOptions.SectionName));

        builder.Services.Configure<NsfwThresholds>(
            builder.Configuration.GetSection(NsfwThresholds.Section));

        return builder;
    }
}
