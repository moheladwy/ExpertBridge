using ExpertBridge.Application.Repositories.Comment;
using ExpertBridge.Application.Repositories.Post;
using ExpertBridge.Application.Repositories.Profile;
using ExpertBridge.Application.Repositories.Tag;
using ExpertBridge.Application.Repositories.User;
using ExpertBridge.Application.Services;
using ExpertBridge.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Core.Entities.Comment;
using ExpertBridge.Core.Entities.Post;
using ExpertBridge.Core.Entities.Profile;
using ExpertBridge.Core.Entities.Tags;
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
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IObjectStorageService, ObjectStorageService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICommentService, CommentService>();
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
        services.AddScoped<ProfileRepository>();
        services.AddScoped<PostRepository>();
        services.AddScoped<CommentRepository>();
        services.AddScoped<TagRepository>();
        services.AddScoped<IEntityRepository<User>, UserCacheRepository>();
        services.AddScoped<IEntityRepository<Profile>, ProfileCacheRepository>();
        services.AddScoped<IEntityRepository<Post>, PostCacheRepository>();
        services.AddScoped<IEntityRepository<Comment>, CommentCacheRepository>();
        services.AddScoped<IEntityRepository<Tag>, TagCacheRepository>();
    }
}
