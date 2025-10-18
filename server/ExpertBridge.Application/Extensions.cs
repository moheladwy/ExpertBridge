// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Application.DomainServices;
using ExpertBridge.Application.EmbeddingService;
using ExpertBridge.Application.Services;
using ExpertBridge.Application.Settings;
using ExpertBridge.Application.Settings.Serilog;
using ExpertBridge.Core.Requests.RegisterUser;
using ExpertBridge.Extensions.AWS;
using ExpertBridge.GroqLibrary.Settings;
using ExpertBridge.Notifications.Extensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExpertBridge.Application;

public static class Extensions
{
    /// <summary>
    ///     Registers domain services and dependencies for the application into the provided <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add the domain services to.</param>
    /// <returns>The <see cref="IServiceCollection" /> with domain services registered.</returns>
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();

        services
            .AddNotifications()
            .AddScoped<S3Service>()
            .AddScoped<IEmbeddingService, OllamaEmbeddingService>()
            .AddScoped<CommentService>()
            .AddScoped<ContentModerationService>()
            .AddScoped<MediaAttachmentService>()
            .AddScoped<TaggingService>()
            .AddScoped<PostService>()
            .AddScoped<JobPostingService>()
            .AddScoped<ProfileService>()
            .AddScoped<JobService>()
            .AddScoped<MessagingService>()
            .AddScoped<GroqPostTaggingService>()
            .AddScoped<GroqTagProcessorService>()
            .AddScoped<GroqInappropriateLanguageDetectionService>()
            ;

        return services;
    }

    /// <summary>
    ///     Configures the settings required by the ExpertBridge application.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance to configure.</param>
    /// <returns>The modified WebApplicationBuilder with the configured application settings.</returns>
    public static TBuilder ConfigureExpertBridgeSettings<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
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
