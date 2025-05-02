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
                .AddGroqHttpClientFactory()
                .AddGroqApiServices()
                ;

            // Background jobs
            builder.AddIpcChannels();
            builder.AddBackgroundWorkers();

            builder.Services.AddServices();

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
                builder.Configuration.GetSection("ConnectionStrings"));

            builder.Services.Configure<FirebaseSettings>(
                builder.Configuration.GetSection("Firebase"));

            builder.Services.Configure<FirebaseAuthSettings>(
                builder.Configuration.GetSection("Authentication:Firebase"));

            builder.Services.Configure<AwsSettings>(
                builder.Configuration.GetSection("AwsS3"));

            builder.Services.Configure<AiSettings>(
                builder.Configuration.GetSection("AI"));

            builder.Services.Configure<SerilogSettings>(
                builder.Configuration.GetSection("Serilog"));

            builder.Services.Configure<ExpertBridgeRateLimitSettings>(
                builder.Configuration.GetSection(ExpertBridgeRateLimitSettings.SectionName));

            builder.Services.Configure<GroqSettings>(builder.Configuration.GetSection(GroqSettings.Section));

            return builder;
        }
    }
}
