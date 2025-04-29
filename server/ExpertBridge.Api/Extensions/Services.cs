// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Api.Application.Interfaces.Repositories;
using ExpertBridge.Api.Application.Interfaces.Services;
using ExpertBridge.Api.Application.Repositories.Comments;
using ExpertBridge.Api.Application.Repositories.Posts;
using ExpertBridge.Api.Application.Repositories.Profiles;
using ExpertBridge.Api.Application.Repositories.Tags;
using ExpertBridge.Api.Application.Repositories.Users;
using ExpertBridge.Api.Application.Services;
using ExpertBridge.Api.BackgroundServices;
using ExpertBridge.Api.Core.Entities.Comments;
using ExpertBridge.Api.Core.Entities.Posts;
using ExpertBridge.Api.Core.Entities.Profiles;
using ExpertBridge.Api.Core.Entities.Tags;
using ExpertBridge.Api.Core.Entities.Users;
using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.HttpClients;
using ExpertBridge.Api.Models;
using ExpertBridge.Api.Requests.RegisterUser;
using ExpertBridge.Api.Services;
using FluentValidation;
using Refit;

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
        //services.AddScoped<IObjectStorageService, ObjectStorageService>();
        //services.AddScoped<IUsersService, UsersService>();
        //services.AddScoped<IProfilesService, ProfilesService>();
        //services.AddScoped<IPostsService, PostsService>();
        services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>()
            .AddTransient<IFirebaseAuthService, FirebaseAuthService>()
            .AddScoped<ICacheService, CacheService>()
            .AddScoped<ICommentsService, CommentsService>()
            .AddScoped<AuthorizationHelper>()
            .AddScoped<S3Service>()
            .AddHostedService<S3CleaningWorker>()
            .AddHostedService<PostCreatedHandlerWorker>()
            .AddHostedService<PeriodicPostTaggingCleanerWorker>()
            .AddSingleton(_ => Channel.CreateUnbounded<PostCreatedMessage>());

        services
            .AddRefitClient<IPostCategroizerClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://categorizer.expertbridge.duckdns.org"));
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
