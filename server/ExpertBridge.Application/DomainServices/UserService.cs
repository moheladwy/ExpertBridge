using ExpertBridge.Application.Helpers;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Core.Exceptions;

namespace ExpertBridge.Application.DomainServices;

/// <summary>
///     Provides high-level user-related operations with integrated authorization and user context management.
/// </summary>
/// <remarks>
///     This service acts as a facade over <see cref="AuthorizationHelper" /> to provide convenient,
///     domain-focused methods for retrieving the current authenticated user and their profile.
///     **Architecture Role:**
///     UserService is a thin wrapper that abstracts authentication complexity from controllers and other services.
///     It provides a clean, semantic API for common user operations while delegating the actual work to
///     AuthorizationHelper.
///     **Authentication Context:**
///     The service operates within ASP.NET Core's authentication pipeline:
///     - HttpContext contains Firebase JWT claims
///     - User.Claims["provider_id"] maps to User.ProviderId in database
///     - IHttpContextAccessor enables authentication access from dependency-injected services
///     **Use Cases:**
///     **1. Controller Actions Requiring Authentication:**
///     <code>
/// public class ProfilesController : ControllerBase
/// {
///     private readonly UserService _userService;
///     
///     [HttpGet("me")]
///     public async Task&lt;IActionResult&gt; GetMyProfile()
///     {
///         var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();
///         return Ok(userProfile);
///     }
/// }
/// </code>
///     **2. Optional Authentication (Anonymous + Authenticated):**
///     <code>
/// [AllowAnonymous]
/// public async Task&lt;IActionResult&gt; GetRecommendedPosts()
/// {
///     var userId = await _userService.GetCurrentUserProfileIdOrEmptyAsync();
///     // userId will be empty string if not authenticated
///     var posts = await _postService.GetRecommendedAsync(userId);
///     return Ok(posts);
/// }
/// </code>
///     **3. Background Workers Needing User Context:**
///     <code>
/// public class NotificationWorker : BackgroundService
/// {
///     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
///     {
///         // Note: Background workers don't have HttpContext
///         // This service won't work in background contexts
///         // Use direct database queries for user lookups instead
///     }
/// }
/// </code>
///     **Key Methods:**
///     **GetCurrentUserPopulatedModelAsync:**
///     - Returns full User entity with populated Profile navigation
///     - Returns null if not authenticated
///     - Use for operations needing full user data (email, roles, etc.)
///     **GetCurrentUserProfileIdOrEmptyAsync:**
///     - Returns profile ID or empty string
///     - Never throws exceptions
///     - Perfect for optional authentication scenarios
///     - Use in [AllowAnonymous] endpoints
///     **GetCurrentUserProfileOrThrowAsync:**
///     - Returns Profile entity or throws UnauthorizedException
///     - Validates authentication and profile existence
///     - Use in [Authorize] endpoints requiring guaranteed authentication
///     - Handles edge cases (no profile, invalid data)
///     **Error Handling:**
///     - UnauthorizedException: User not authenticated or profile missing
///     - UserNotFoundException: User exists but profile not found in database (data integrity issue)
///     **Performance Considerations:**
///     - Database query executed on each call (no caching)
///     - Include navigation properties loaded (User.Profile)
///     - For high-frequency calls, consider caching in caller
///     **Security Notes:**
///     - Never trust client-provided user IDs
///     - Always use GetCurrentUserProfileOrThrowAsync for sensitive operations
///     - Profile ID from this service is guaranteed to match authenticated user
///     **Data Integrity:**
///     If GetCurrentUserProfileOrThrowAsync throws UserNotFoundException while user is authenticated,
///     this indicates a database consistency issue requiring investigation:
///     - User record exists without Profile
///     - Profile was soft-deleted but User wasn't
///     - Migration didn't create Profile for existing User
///     The service is registered as scoped in the DI container, sharing lifetime with the HTTP request
///     and maintaining consistent authentication context throughout the request pipeline.
/// </remarks>
public class UserService
{
    private readonly AuthorizationHelper _authHelper;

    /// <summary>
    ///     Initializes a new instance of the <see cref="UserService" /> class.
    /// </summary>
    /// <param name="authHelper">
    ///     The authorization helper providing authentication context and user retrieval capabilities.
    /// </param>
    public UserService(AuthorizationHelper authHelper)
    {
        _authHelper = authHelper;
    }

    /// <summary>
    ///     Retrieves the fully populated User entity for the currently authenticated user, including their Profile.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the authenticated <see cref="User" />
    ///     with populated Profile navigation property, or null if no user is authenticated.
    /// </returns>
    /// <remarks>
    ///     **Use Cases:**
    ///     - Operations requiring full user data (email, provider ID, roles)
    ///     - Admin dashboards showing user details
    ///     - Audit logging with complete user information
    ///     - Email notification personalization
    ///     **Included Navigation Properties:**
    ///     - User.Profile (FirstName, LastName, Bio, Skills, etc.)
    ///     **Database Query:**
    ///     <code>
    /// SELECT * FROM Users u
    /// INNER JOIN Profiles p ON u.ProfileId = p.Id
    /// WHERE u.ProviderId = @providerIdFromClaims
    /// </code>
    ///     **Return Value:**
    ///     - User entity with Profile: User is authenticated and found in database
    ///     - null: No authenticated user (anonymous request) or user not found
    ///     **Example:**
    ///     <code>
    /// var user = await userService.GetCurrentUserPopulatedModelAsync();
    /// if (user != null)
    /// {
    ///     logger.LogInformation("User {Email} accessed endpoint", user.Email);
    ///     await SendEmailAsync(user.Email, user.Profile.FirstName);
    /// }
    /// </code>
    ///     **Performance:**
    ///     - Single database query with eager loading
    ///     - No caching (fresh data on each call)
    ///     - Consider caching in caller for high-frequency operations
    ///     This method never throws exceptions - returns null for unauthenticated requests.
    /// </remarks>
    public async Task<User?> GetCurrentUserPopulatedModelAsync()
    {
        return await _authHelper.GetCurrentUserAsync();
    }

    /// <summary>
    ///     Retrieves the Profile ID of the currently authenticated user, or empty string if not authenticated.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the Profile ID string if authenticated,
    ///     or <see cref="string.Empty" /> if no user is authenticated or profile doesn't exist.
    /// </returns>
    /// <remarks>
    ///     **Use Cases:**
    ///     - Optional authentication endpoints ([AllowAnonymous])
    ///     - Personalized content for authenticated users, generic for anonymous
    ///     - Analytics tracking with optional user identification
    ///     - Filtering content based on user preferences when available
    ///     **Return Values:**
    ///     - Profile ID (GUID string): User authenticated and profile exists
    ///     - Empty string: Anonymous request, authentication failed, or no profile
    ///     **Example Usage:**
    ///     <code>
    /// [AllowAnonymous]
    /// [HttpGet("feed")]
    /// public async Task&lt;IActionResult&gt; GetFeed()
    /// {
    ///     var userProfileId = await _userService.GetCurrentUserProfileIdOrEmptyAsync();
    ///     
    ///     // Personalized recommendations if authenticated
    ///     var posts = await _postService.GetRecommendedPostsAsync(userProfileId);
    ///     
    ///     return Ok(posts);
    /// }
    /// </code>
    ///     **Security Characteristics:**
    ///     - Never throws exceptions (safe for any authentication state)
    ///     - Empty string is safe sentinel value (not null, no GUID parsing errors)
    ///     - Can be used in database queries (WHERE ProfileId = @emptyString returns no rows)
    ///     **Performance:**
    ///     - Same database query as GetCurrentUserPopulatedModelAsync
    ///     - Returns immediately if no authentication
    ///     - Minimal memory allocation (string result only)
    ///     **Alternative Pattern:**
    ///     For null-safe handling, consider:
    ///     <code>
    /// var userId = await _userService.GetCurrentUserProfileIdOrEmptyAsync();
    /// if (!string.IsNullOrEmpty(userId))
    /// {
    ///     // User is authenticated, use userId
    /// }
    /// </code>
    ///     This method is ideal for endpoints that support both authenticated and anonymous access.
    /// </remarks>
    public async Task<string> GetCurrentUserProfileIdOrEmptyAsync()
    {
        var user = await _authHelper.GetCurrentUserAsync();
        return user?.Profile?.Id ?? string.Empty;
    }

    /// <summary>
    ///     Retrieves the Profile of the currently authenticated user, throwing an exception if not authenticated or profile
    ///     not found.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the authenticated user's <see cref="Profile" />.
    /// </returns>
    /// <exception cref="UnauthorizedException">
    ///     Thrown when the user is not authenticated, or the authenticated user has no associated profile.
    /// </exception>
    /// <exception cref="UserNotFoundException">
    ///     Thrown when the profile exists in the User entity but not found in the database (data integrity issue).
    /// </exception>
    /// <remarks>
    ///     **Use Cases:**
    ///     - Endpoints requiring guaranteed authentication ([Authorize])
    ///     - Creating content (posts, comments) needing author information
    ///     - Profile update operations
    ///     - Operations that must fail if not authenticated
    ///     **Validation Flow:**
    ///     1. Check if user is authenticated → throw UnauthorizedException if not
    ///     2. Check if user has profile ID → throw UnauthorizedException if missing
    ///     3. Check if profile exists in database → throw UserNotFoundException if missing
    ///     4. Return profile if all checks pass
    ///     **Example Usage:**
    ///     <code>
    /// [Authorize]
    /// [HttpPost("posts")]
    /// public async Task&lt;IActionResult&gt; CreatePost(CreatePostRequest request)
    /// {
    ///     var authorProfile = await _userService.GetCurrentUserProfileOrThrowAsync();
    ///     var post = await _postService.CreateAsync(request, authorProfile);
    ///     return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    /// }
    /// </code>
    ///     **Error Scenarios:**
    ///     **1. Not Authenticated:**
    ///     - JWT token missing/invalid/expired
    ///     - Throws UnauthorizedException: "User is not authenticated."
    ///     - HTTP 401 Unauthorized
    ///     **2. Profile ID Missing:**
    ///     - User exists but Profile navigation is null or has no ID
    ///     - Indicates incomplete user registration or data corruption
    ///     - Throws UnauthorizedException: "User profile identifier is missing."
    ///     - Should trigger investigation - users should always have profiles
    ///     **3. Profile Not Found:**
    ///     - User.ProfileId exists but no matching Profile in database
    ///     - Data integrity issue (orphaned reference, deleted profile)
    ///     - Throws UserNotFoundException: "User profile with id={id} was not found."
    ///     - Requires database consistency check
    ///     **Security Guarantees:**
    ///     - Returned Profile always belongs to authenticated user
    ///     - No privilege escalation possible
    ///     - Profile ID cannot be spoofed by client
    ///     - Authorization validated by ASP.NET Core middleware
    ///     **Logging Considerations:**
    ///     The method includes commented logging statements for production use:
    ///     <code>
    /// Serilog.Log.Error("Authenticated user {UserId} has no associated ProfileId.", user.Id);
    /// Serilog.Log.Error("Profile not found for ProfileId {ProfileId} of User {UserId}.", profileId, userId);
    /// </code>
    ///     **Performance:**
    ///     - Single database query (User with included Profile)
    ///     - Throws exceptions immediately without retry
    ///     - No caching (ensures fresh authorization state)
    ///     **Global Exception Handling:**
    ///     Exceptions thrown by this method are caught by GlobalExceptionMiddleware:
    ///     - UnauthorizedException → 401 Unauthorized response
    ///     - UserNotFoundException → 404 Not Found response
    ///     Always use this method in [Authorize] endpoints where authentication is mandatory.
    /// </remarks>
    public async Task<Profile> GetCurrentUserProfileOrThrowAsync()
    {
        var user = await _authHelper.GetCurrentUserAsync();

        if (user == null)
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        if (string.IsNullOrEmpty(user.Profile?.Id))
        {
            // This case implies data integrity issue or incomplete user setup
            // Log this as a server-side issue
            // Serilog.Log.Error("Authenticated user {UserId} has no associated ProfileId.", user.Id);
            throw new UnauthorizedException("User profile identifier is missing.");
        }

        if (user.Profile == null)
        {
            // Serilog.Log.Error("Profile not found in database for ProfileId {ProfileId} associated with User {UserId}.", user.Profile.Id, user.Id);
            throw new UserNotFoundException(
                $"User profile with id={user.Profile.Id} was not found."); // Or a generic Unauthorized
        }

        return user.Profile;
    }
}
