using ExpertBridge.Application.DomainServices;
using ExpertBridge.Application.Helpers;

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
                .AddScoped<JobPostingService>()
                .AddScoped<ProfileService>()
                .AddScoped<JobService>()
                .AddScoped<MessagingService>()
                ;

            return services;
        }
    }
}
