// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Settings.Serilog;
using ExpertBridge.Api.Settings;
using ExpertBridge.Data;
using ExpertBridge.GroqLibrary.Settings;

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
            builder.AddRedisDistributedCache(connectionName: "Redis");
            builder.AddS3ObjectService();
            builder.AddRateLimiting();

            // Firebase
            builder.AddFirebaseApp();
            builder.AddFirebaseAuthentication();
            builder.AddHttpClientForFirebaseService();

            // External remote services
            builder.AddEmbeddingServices();
            builder.AddRefitHttpClients();
            builder
                .AddGroqApiServices()
                ;

            // Background jobs
            builder.AddIpcChannels();
            builder.AddBackgroundWorkers();

            builder.Services.AddServices();

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
