namespace ExpertBridge.Application.Settings;

/// <summary>
///     Defines cache profile names used throughout the application for consistent caching strategies.
/// </summary>
/// <remarks>
///     Cache profiles configure response caching behavior including duration, location, and variance.
///     These profiles are referenced in controller attributes and middleware configuration.
///     **Usage in Controllers:**
///     <code>
/// [ResponseCache(CacheProfile = CacheProfiles.PersonalizedContent)]
/// public async Task&lt;IActionResult&gt; GetUserFeed() { }
/// 
/// [ResponseCache(CacheProfile = CacheProfiles.Default)]
/// public async Task&lt;IActionResult&gt; GetPublicPosts() { }
/// </code>
///     **Configuration in Program.cs:**
///     <code>
/// builder.Services.AddControllers()
///     .AddMvcOptions(options =>
///     {
///         options.CacheProfiles.Add(CacheProfiles.PersonalizedContent, new CacheProfile
///         {
///             Duration = 60,
///             VaryByQueryKeys = new[] { "userId" },
///             Location = ResponseCacheLocation.Client
///         });
///         options.CacheProfiles.Add(CacheProfiles.Default, new CacheProfile
///         {
///             Duration = 300,
///             Location = ResponseCacheLocation.Any
///         });
///     });
/// </code>
///     Using named constants ensures consistency and prevents typos in cache profile references.
/// </remarks>
public static class CacheProfiles
{
    /// <summary>
    ///     Cache profile for user-specific, personalized content that varies by user identity.
    /// </summary>
    /// <remarks>
    ///     Used for endpoints returning data tailored to the authenticated user (e.g., personalized feeds, recommendations).
    ///     Typically has shorter duration and varies by user-specific parameters.
    /// </remarks>
    public const string PersonalizedContent = "PersonalizedContent";

    /// <summary>
    ///     Default cache profile for general, non-personalized content.
    /// </summary>
    /// <remarks>
    ///     Used for public endpoints returning the same data to all users (e.g., public posts, job listings).
    ///     Typically has longer duration and can be cached at any location (client, proxy, server).
    /// </remarks>
    public const string Default = "Default";
}
