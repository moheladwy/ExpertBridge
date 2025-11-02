using ExpertBridge.Api.Services;
using ExpertBridge.Application.DomainServices;
using ExpertBridge.Application.EmbeddingService;
using ExpertBridge.Application.Helpers;
using ExpertBridge.Application.Services;
using ExpertBridge.Contract.Requests.RegisterUser;
using FluentValidation;

namespace ExpertBridge.Api.Extensions;

/// <summary>
///     Provides extension methods for registering domain services and their dependencies
///     into the application's dependency injection system.
/// </summary>
internal static class DomainServices
{
    /// <summary>
    ///     Registers domain services and dependencies for the application into the provided <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add the domain services to.</param>
    /// <returns>The <see cref="IServiceCollection" /> with domain services registered.</returns>
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>()
            .AddScoped<AuthorizationHelper>()
            .AddScoped<S3Service>()
            ;

        services.AddSingleton<IEmbeddingService, OllamaEmbeddingService>();
        services
            .AddScoped<CommentService>()
            .AddScoped<ContentModerationService>()
            .AddScoped<MediaAttachmentService>()
            .AddScoped<TaggingService>()
            .AddScoped<UserService>()
            .AddScoped<AuthorizationHelper>()
            .AddScoped<PostService>()
            .AddScoped<JobPostingService>()
            .AddScoped<ProfileService>()
            .AddScoped<JobService>()
            .AddScoped<MessagingService>()
            .AddScoped<MediaService>()
            .AddScoped<SearchService>()
            ;

        return services;
    }
}
