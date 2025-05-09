using ExpertBridge.Core.Interfaces.Services;
using ExpertBridge.Core.Requests.RegisterUser;
using ExpertBridge.Api.EmbeddingService;
using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Services;
using FluentValidation;
using ExpertBridge.Notifications;
using ExpertBridge.Api.DomainServices;

namespace ExpertBridge.Api.Extensions;

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
            .AddScoped<S3Service>()
            ;

        services.AddSingleton<IEmbeddingService, OllamaEmbeddingService>();
    }
}
