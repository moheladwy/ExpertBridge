// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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

/// <summary>
///     Provides extension methods for configuring ExpertBridge application services and settings.
/// </summary>
public static class Extensions
{
    /// <summary>
    ///     Registers all domain services, validators, and application dependencies into the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the domain services to.</param>
    /// <returns>The service collection with all domain services registered for method chaining.</returns>
    /// <remarks>
    ///     This extension method configures the complete application layer including:
    ///     **Validators:**
    ///     - FluentValidation validators from Core.Requests assembly (e.g., RegisterUserRequestValidator)
    ///     **Infrastructure Services:**
    ///     - Notification system (SignalR, background workers, channel pipeline)
    ///     - S3Service: AWS S3 integration for file storage and media management
    ///     - IEmbeddingService: Ollama-based vector embedding generation for AI-powered search
    ///     **Domain Services:**
    ///     - CommentService: Comment creation, editing, deletion, and vote management
    ///     - ContentModerationService: AI-powered content moderation using Groq LLM
    ///     - MediaAttachmentService: Media upload, validation, and S3 storage
    ///     - TaggingService: Tag extraction and management for posts and profiles
    ///     - PostService: Post CRUD operations and recommendation algorithms
    ///     - JobPostingService: Job posting management and matching
    ///     - ProfileService: User profile management and skill tracking
    ///     - JobService: Job application and offer workflow
    ///     - MessagingService: Real-time chat and messaging functionality
    ///     **AI/LLM Services (Groq Integration):**
    ///     - AiPostTaggingService: Automatic tag generation from post content using Groq LLM
    ///     - AiTagProcessorService: Tag normalization and validation
    ///     - NsfwContentDetectionService: Content safety analysis using Groq LLM
    ///     All services are registered with scoped lifetime for proper database context management and request isolation.
    ///     Typical usage in Program.cs:
    ///     <code>
    /// builder.Services.AddDomainServices();
    /// </code>
    /// </remarks>
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();

        services
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
            .AddScoped<AiPostTaggingService>()
            .AddScoped<AiTagProcessorService>()
            .AddScoped<NsfwContentDetectionService>()
            ;

        return services;
    }

    /// <summary>
    ///     Configures all application settings from appsettings.json into strongly-typed configuration objects.
    /// </summary>
    /// <typeparam name="TBuilder">The host application builder type (WebApplicationBuilder, HostApplicationBuilder, etc.).</typeparam>
    /// <param name="builder">The host application builder instance to configure.</param>
    /// <returns>The modified builder with all application settings configured for method chaining.</returns>
    /// <remarks>
    ///     This extension method binds configuration sections to strongly-typed settings classes using the Options pattern.
    ///     All settings are registered with IOptions&lt;T&gt; for dependency injection.
    ///     **Configured Settings:**
    ///     **Database:**
    ///     - ConnectionStrings: PostgreSQL connection strings for main database and read replicas
    ///     **Authentication:**
    ///     - FirebaseSettings: Firebase project configuration (ApiKey, ProjectId, AuthDomain)
    ///     - FirebaseAuthSettings: Firebase authentication settings and JWT validation
    ///     **Cloud Storage:**
    ///     - AwsSettings: AWS credentials, region, and S3 bucket configuration for media storage
    ///     **AI/Machine Learning:**
    ///     - AiSettings: Ollama embedding service configuration (base URL, model selection)
    ///     - GroqSettings: Groq LLM API settings (API key, model selection, temperature, max tokens)
    ///     - InappropriateLanguageThresholds: Content moderation sensitivity thresholds for different categories
    ///     **Logging:**
    ///     - SerilogSettings: Structured logging configuration (log levels, sinks, enrichment)
    ///     **Rate Limiting:**
    ///     - ExpertBridgeRateLimitSettings: API rate limiting policies (requests per minute, burst limits)
    ///     Each settings class defines its own Section constant for configuration binding.
    ///     Settings are validated at startup through data annotations and custom validators.
    ///     Typical usage in Program.cs:
    ///     <code>
    /// builder.ConfigureExpertBridgeSettings();
    /// </code>
    /// </remarks>
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
