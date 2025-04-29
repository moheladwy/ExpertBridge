using ExpertBridge.Api.BackgroundServices;
using ExpertBridge.Api.EmbeddingService;
using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Services;
using ExpertBridge.Core.Interfaces.Services;
using ExpertBridge.Core.Requests.RegisterUser;
using FluentValidation;

namespace ExpertBridge.Api.Extensions;

public static class Services
{
    /// <summary>
    ///     Adds the services to the services collection.
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
            .AddHostedService<S3CleaningWorker>()
            .AddHostedService<PostCreatedHandlerWorker>()
            .AddScoped<TaggingService>()
            ;
        services.AddSingleton<IEmbeddingService, OllamaEmbeddingService>();
    }
}
