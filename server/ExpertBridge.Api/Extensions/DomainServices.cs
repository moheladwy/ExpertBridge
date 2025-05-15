// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.DomainServices;
using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Services;

namespace ExpertBridge.Api.Extensions
{
    public static class DomainServices
    {
        public static IServiceCollection
        AddDomainServices(this IServiceCollection services)
        {
            services
                .AddScoped<CommentService>()
                .AddScoped<ContentModerationService>()
                .AddScoped<MediaAttachmentService>()
                .AddScoped<TaggingService>()
                .AddScoped<UserService>()
                .AddScoped<AuthorizationHelper>()
                .AddScoped<PostService>()
                ;

            return services;
        }
    }
}
