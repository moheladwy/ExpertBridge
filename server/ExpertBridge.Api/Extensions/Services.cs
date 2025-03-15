using ExpertBridge.Api.Application.Repositories.Comment;
using ExpertBridge.Api.Application.Repositories.Post;
using ExpertBridge.Api.Application.Repositories.Profile;
using ExpertBridge.Api.Application.Repositories.Tag;
using ExpertBridge.Api.Application.Repositories.User;
using ExpertBridge.Api.Application.Services;
using ExpertBridge.Api.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Api.Core.Entities.Comment;
using ExpertBridge.Api.Core.Entities.Post;
using ExpertBridge.Api.Core.Entities.Profile;
using ExpertBridge.Api.Core.Entities.Tags;
using ExpertBridge.Api.Core.Entities.User;
using ExpertBridge.Api.Core.Interfaces.Repositories;
using ExpertBridge.Api.Core.Interfaces.Services;
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
