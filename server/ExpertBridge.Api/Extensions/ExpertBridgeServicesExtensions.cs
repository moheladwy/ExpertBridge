using System.Text.Json;
using ExpertBridge.Application.Settings;
using ExpertBridge.Application.Settings.Serilog;
using ExpertBridge.Data;
using ExpertBridge.Extensions.Caching;
using ExpertBridge.GroqLibrary.Settings;
using Polly;
using Polly.Retry;
using Serilog;

namespace ExpertBridge.Api.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring and adding services required by the ExpertBridge application.
    /// </summary>
    public static class ExpertBridgeServicesExtensions
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
            builder.AddSeqEndpoint(connectionName: "Seq");
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
            builder
                .AddGroqApiServices()
                ;

            // Background jobs
            builder.AddIpcChannels();
            builder.AddBackgroundWorkers();

            builder.Services.AddServices();
            builder.Services.AddDomainServices();

            builder.Services.AddResiliencePipeline(ResiliencePipelines.MalformedJsonModelResponse, static builder =>
            {
                // See: https://www.pollydocs.org/strategies/retry.html
                builder.AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<JsonException>(),
                    MaxRetryAttempts = 5,
                    Delay = TimeSpan.FromSeconds(2),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = context =>
                    {
                        // Log the retry attempt
                        Log.Information("Retrying due to a malformed json in model response. Attempt");
                        return ValueTask.CompletedTask;
                    }
                });

                // See: https://www.pollydocs.org/strategies/timeout.html
                builder.AddTimeout(TimeSpan.FromSeconds(90));
            });



            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            return builder;
        }

        /// <summary>
        /// Configures the settings required by the ExpertBridge application.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder instance to configure.</param>
        /// <returns>The modified WebApplicationBuilder with the configured application settings.</returns>
        private static WebApplicationBuilder ConfigureExpertBridgeSettings(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<ConnectionStrings>(
                builder.Configuration.GetSection(ConnectionStrings.Section));

            builder.Services.Configure<FirebaseSettings>(
                builder.Configuration.GetSection(FirebaseSettings.Section));

            builder.Services.Configure<FirebaseAuthSettings>(
                builder.Configuration.GetSection(FirebaseAuthSettings.Section));

            builder.Services.Configure<AwsSettings>(
                builder.Configuration.GetSection(AwsSettings.Section));

            builder.Services.Configure<AiSettings>(
                builder.Configuration.GetSection(AiSettings.Section));

            builder.Services.Configure<SerilogSettings>(
                builder.Configuration.GetSection(SerilogSettings.Section));

            builder.Services.Configure<ExpertBridgeRateLimitSettings>(
                builder.Configuration.GetSection(ExpertBridgeRateLimitSettings.SectionName));

            builder.Services.Configure<InappropriateLanguageThresholds>(
                builder.Configuration.GetSection(InappropriateLanguageThresholds.Section));

            builder.Services.Configure<GroqSettings>(builder.Configuration.GetSection(GroqSettings.Section));

            return builder;
        }
    }
}
