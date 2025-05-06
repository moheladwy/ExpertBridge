using Api.EmbeddingService;
using Api.Helpers;
using Api.Services;
using Core.Interfaces.Services;
using Core.Requests.RegisterUser;
using FluentValidation;

namespace Api.Extensions;

/// <summary>
///     Provides extension methods for adding services to the dependency injection container.
/// </summary>
public static class Services
{
    /// <summary>
    ///     Adds the services to the service's collection.
    /// </summary>
    /// <param name="services">
    ///     The service collection to add the services to.
    /// </param>
    public static void AddServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>()
            .AddTransient<IFirebaseAuthService, FirebaseAuthService>()
            .AddScoped<ICacheService, CacheService>()
            .AddScoped<AuthorizationHelper>()
            .AddScoped<S3Service>()
            .AddScoped<TaggingService>()
            .AddScoped<ContentModerationService>()
            .AddScoped<NotificationFacade>()
            ;

        services.AddSingleton<IEmbeddingService, OllamaEmbeddingService>();
    }
}
