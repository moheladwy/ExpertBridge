using ExpertBridge.Application.Repositories.Comment;
using ExpertBridge.Application.Repositories.Post;
using ExpertBridge.Application.Repositories.Profile;
using ExpertBridge.Application.Repositories.User;
using ExpertBridge.Application.Services;
using ExpertBridge.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Core.Entities.Comment;
using ExpertBridge.Core.Entities.Post;
using ExpertBridge.Core.Entities.Profile;
using ExpertBridge.Core.Entities.User;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Core.Interfaces.Services;
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
        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<IObjectStorageService, ObjectStorageService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IProfileService, ProfileService>();
        services.AddSingleton<IPostService, PostService>();
        services.AddSingleton<ICommentService, CommentService>();
    }

    /// <summary>
    ///     Adds the repositories to the services collection.
    /// </summary>
    /// <param name="services">
    ///     The service collection to add the repositories to.
    /// </param>
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<UserRepository>();
        services.AddSingleton<ProfileRepository>();
        services.AddSingleton<PostRepository>();
        services.AddSingleton<CommentRepository>();
        services.AddSingleton<IEntityRepository<User>, UserCacheRepository>();
        services.AddSingleton<IEntityRepository<Profile>, ProfileCacheRepository>();
        services.AddSingleton<IEntityRepository<Post>, PostCacheRepository>();
        services.AddSingleton<IEntityRepository<Comment>, CommentCacheRepository>();
    }
}
