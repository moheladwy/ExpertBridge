using System.Threading.RateLimiting;
using ExpertBridge.Application.Settings;

namespace ExpertBridge.Api.Extensions;

/// <summary>
///     Provides extension methods for configuring rate limiting in an application.
/// </summary>
internal static class RateLimiting
{
    /// Adds rate limiting functionality to the application's service collection using the specified settings.
    /// <param name="builder">
    ///     The application builder to configure rate limiting for.
    /// </param>
    /// <returns>
    ///     The updated service collection with rate limiting configured.
    /// </returns>
    public static WebApplicationBuilder AddRateLimiting(this WebApplicationBuilder builder)
    {
        var rateLimitOptions = new RateLimitOptions();
        builder.Configuration.GetSection(RateLimitOptions.SectionName).Bind(rateLimitOptions);

        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    partition => new FixedWindowRateLimiterOptions
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

                await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.",
                    cancellationToken);
            };
        });

        return builder;
    }
}
