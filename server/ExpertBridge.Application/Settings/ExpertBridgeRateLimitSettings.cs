namespace ExpertBridge.Application.Settings;

/// <summary>
///     Configuration settings for API rate limiting to prevent abuse and ensure fair resource usage.
/// </summary>
/// <remarks>
///     This settings class configures the rate limiting policy applied to API endpoints.
///     Uses the fixed window algorithm to limit the number of requests per user/IP within a time window.
///     **Configured in appsettings.json under "RateLimit" section:**
///     <code>
/// {
///   "RateLimit": {
///     "PermitLimit": 100,
///     "Window": 60,
///     "QueueLimit": 10
///   }
/// }
/// </code>
///     **Rate Limiting Algorithm:**
///     - Fixed Window: Resets counter at fixed intervals (e.g., every 60 seconds)
///     - Requests are tracked per authenticated user or IP address
///     - Exceeded requests return 429 Too Many Requests status
///     **Configuration Guidelines:**
///     - PermitLimit: Balance between user experience and server protection (50-200 for APIs)
///     - Window: Shorter windows (30-60s) provide better burst protection
///     - QueueLimit: Prevents thundering herd during high load (5-20 typical)
///     **Typical Values:**
///     - Development: 1000 permits/60s window (lenient for testing)
///     - Production: 100 permits/60s window (balance)
///     - Public APIs: 60 permits/60s window (strict)
///     Applied via ASP.NET Core RateLimiting middleware in Program.cs.
/// </remarks>
public sealed class ExpertBridgeRateLimitSettings
{
    /// <summary>
    ///     The configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "RateLimit";

    /// <summary>
    ///     Gets or sets the maximum number of requests allowed within the time window.
    /// </summary>
    /// <remarks>
    ///     Once this limit is reached, subsequent requests are rejected with HTTP 429
    ///     until the window resets. Recommended: 50-200 for typical web APIs.
    /// </remarks>
    public int PermitLimit { get; set; }

    /// <summary>
    ///     Gets or sets the time window duration in seconds for the rate limit counter.
    /// </summary>
    /// <remarks>
    ///     The counter resets to zero after this duration. Common values: 30, 60, or 300 seconds.
    ///     Shorter windows provide better burst protection but may impact legitimate users.
    /// </remarks>
    public int Window { get; set; }

    /// <summary>
    ///     Gets or sets the maximum number of queued requests when the permit limit is reached.
    /// </summary>
    /// <remarks>
    ///     When permits are exhausted, incoming requests are queued up to this limit.
    ///     Queued requests are processed when the window resets or permits become available.
    ///     Set to 0 to reject immediately without queueing. Recommended: 5-20 for graceful degradation.
    /// </remarks>
    public int QueueLimit { get; set; }
}
