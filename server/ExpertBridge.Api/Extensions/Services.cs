using ExpertBridge.Application.EmbeddingService;
using ExpertBridge.Application.Helpers;
using ExpertBridge.Application.Services;
using ExpertBridge.Core.Requests.RegisterUser;
using FluentValidation;

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
            .AddScoped<AuthorizationHelper>()
            .AddScoped<S3Service>()
            ;

        // services.AddSingleton<IEmbeddingService, OllamaEmbeddingService>();
        services.AddSingleton<IEmbeddingService, QueuedEmbeddingService>();
    }
}
