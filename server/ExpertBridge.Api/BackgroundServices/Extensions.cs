using ExpertBridge.Api.BackgroundServices.PostTagging;

namespace ExpertBridge.Api.BackgroundServices;

internal static class Extensions
{
    public static void AddBackgroundServices(this IServiceCollection services, IConfiguration configuration)
    {
        var postTaggingServiceUrl = configuration["AI:PostCategorizationUrl"]!;
        services.AddHttpClient<PostTaggingBackgroundService>(client =>
        {
            client.BaseAddress = new Uri(postTaggingServiceUrl);
            client.Timeout = TimeSpan.FromMinutes(5);
        });
    }
}
