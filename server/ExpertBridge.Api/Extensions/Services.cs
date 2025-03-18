// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Application.Repositories.Comments;
using ExpertBridge.Api.Application.Repositories.Posts;
using ExpertBridge.Api.Application.Repositories.Profiles;
using ExpertBridge.Api.Application.Repositories.Tags;
using ExpertBridge.Api.Application.Repositories.Users;
using ExpertBridge.Api.Application.Services;
using ExpertBridge.Api.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Api.Core.Entities.Comments;
using ExpertBridge.Api.Core.Entities.Posts;
using ExpertBridge.Api.Core.Entities.Profiles;
using ExpertBridge.Api.Core.Entities.Tags;
using ExpertBridge.Api.Core.Entities.Users;
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
        services.AddScoped<UsersRepository>();
        services.AddScoped<ProfilesRepository>();
        services.AddScoped<PostsRepository>();
        services.AddScoped<CommentsRepository>();
        services.AddScoped<TagsRepository>();
        services.AddScoped<IEntityRepository<User>, UsersCacheRepository>();
        services.AddScoped<IEntityRepository<Profile>, ProfilesCacheRepository>();
        services.AddScoped<IEntityRepository<Post>, PostsCacheRepository>();
        services.AddScoped<IEntityRepository<Comment>, CommentsCacheRepository>();
        services.AddScoped<IEntityRepository<Tag>, TagsCacheRepository>();
    }
}
