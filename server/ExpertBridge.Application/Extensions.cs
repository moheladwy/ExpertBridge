using ExpertBridge.Application.CachedRepositories;
using ExpertBridge.Application.Services;
using ExpertBridge.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Core.Entities.User;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Core.Interfaces.Services;
using ExpertBridge.Data.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ExpertBridge.Application;

public static class Extensions
{
    /// <summary>
    ///     Adds the services to the services collection.
    /// </summary>
    /// <param name="services">
    ///     The service collection to add the services to.
    /// </param>
    public static void AddServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
        services.AddTransient<IFirebaseService, FirebaseService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICacheService, RedisService>();
        services.AddScoped<IObjectStorageService, ObjectStorageService>();
    }

    /// <summary>
    ///     Adds the repositories to the services collection.
    /// </summary>
    /// <param name="services">
    ///     The service collection to add the repositories to.
    /// </param>
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<UserRepository>();
    }

    /// <summary>
    ///     Adds the cached repositories to the services collection.
    /// </summary>
    /// <param name="services">
    ///     The service collection to add the cached repositories to.
    /// </param>
    public static void AddCachedRepositories(this IServiceCollection services)
    {
        services.AddScoped<IEntityRepository<User>, UserCacheRepository>();
    }
}
