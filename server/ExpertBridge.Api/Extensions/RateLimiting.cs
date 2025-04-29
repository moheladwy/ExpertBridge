

using System.Threading.RateLimiting;
using ExpertBridge.Api.Settings;

namespace ExpertBridge.Api.Extensions
{
    public static class RateLimiting
    {
        public static IServiceCollection AddRateLimiting(
            this IServiceCollection services,
            ExpertBridgeRateLimitSettings rateLimitOptions)
        {
            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            PermitLimit = rateLimitOptions.PermitLimit,
                            QueueLimit = rateLimitOptions.QueueLimit,
                            Window = TimeSpan.FromSeconds(rateLimitOptions.Window)
                        }));

                options.OnRejected = async (context, cancellationToken) =>
                {
                    // Custom rejection handling logic
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.Headers["Retry-After"] = "60";

                    await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", cancellationToken);
                };
            });

            return services;
        }
    }
}
