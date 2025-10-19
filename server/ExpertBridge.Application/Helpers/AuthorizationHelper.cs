using System.Security.Claims;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Application.Helpers;

/// <summary>
/// Provides helper methods for authorization and user context management in API requests.
/// </summary>
/// <remarks>
/// This helper class simplifies accessing the currently authenticated user from the HTTP context
/// throughout the application layer, particularly in domain services that need user context.
/// 
/// **Key Responsibilities:**
/// - Extract authenticated user from JWT claims
/// - Load user entity with navigation properties from database
/// - Provide consistent user retrieval pattern across services
/// 
/// **Authentication Flow:**
/// 1. JWT middleware validates Firebase ID token
/// 2. User claims are populated in HttpContext.User
/// 3. AuthorizationHelper extracts email claim
/// 4. User entity is loaded from database with Profile relationship
/// 5. Domain services use the loaded user for authorization checks
/// 
/// **Use Cases:**
/// - Domain services needing to verify resource ownership
/// - Controllers requiring current user context
/// - Authorization logic in business operations
/// - Audit logging with user information
/// 
/// **Design Pattern:**
/// This is a scoped service providing access to per-request authentication context.
/// It encapsulates the complexity of claim extraction and database lookup,
/// providing a clean API for services to get the current user.
/// 
/// **Typical Usage in Services:**
/// <code>
/// public class PostService
/// {
///     private readonly AuthorizationHelper _authHelper;
///     
///     public async Task&lt;Post&gt; GetMyPostAsync(string postId)
///     {
///         var currentUser = await _authHelper.GetCurrentUserAsync();
///         if (currentUser == null) throw new UnauthorizedException();
///         
///         var post = await _dbContext.Posts.FindAsync(postId);
///         if (post.AuthorId != currentUser.Profile.Id)
///             throw new ForbiddenException();
///         
///         return post;
///     }
/// }
/// </code>
/// 
/// Should be registered as scoped service to match HTTP request lifetime.
/// </remarks>
public class AuthorizationHelper
{
    /// <summary>
    /// The database context for querying user entities.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    /// The HTTP context accessor for retrieving authentication claims from the current request.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationHelper"/> class with database context and HTTP context accessor.
    /// </summary>
    /// <param name="dbContext">
    /// The database context for loading user entities.
    /// </param>
    /// <param name="httpContextAccessor">
    /// The HTTP context accessor for accessing authentication claims from the current request.
    /// </param>
    /// <remarks>
    /// Both dependencies are injected as scoped services, ensuring they're tied to the HTTP request lifetime.
    /// IHttpContextAccessor must be registered in DI: services.AddHttpContextAccessor()
    /// </remarks>
    public AuthorizationHelper(
        ExpertBridgeDbContext dbContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Retrieves the currently authenticated user from the database with the Profile navigation property populated.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation, containing:
    /// - The <see cref="User"/> entity with Profile loaded if the user is authenticated
    /// - null if no user is authenticated or authentication validation failed
    /// </returns>
    /// <remarks>
    /// This method performs the following steps:
    /// 
    /// **1. Claim Extraction:**
    /// - Accesses HttpContext.User from the current HTTP request
    /// - Extracts the email claim (ClaimTypes.Email) set by JWT authentication middleware
    /// - Returns null if HttpContext, User, or email claim is not available
    /// 
    /// **2. Database Query:**
    /// - Queries Users table filtering by email
    /// - Includes Profile navigation property for immediate access
    /// - Uses FirstOrDefaultAsync for efficient single-record retrieval
    /// - Returns null if no matching user found
    /// 
    /// **3. Result:**
    /// - User entity with Profile fully loaded (eager loading via Include)
    /// - Allows immediate access to user.Profile without lazy loading issues
    /// - No tracking by default (can be modified if needed)
    /// 
    /// **Authentication Requirements:**
    /// - Endpoint must be protected with [Authorize] attribute
    /// - Valid Firebase JWT token must be present in Authorization header
    /// - JWT must have been successfully validated by authentication middleware
    /// - Email claim must be present in token (set by Firebase)
    /// 
    /// **Null Return Scenarios:**
    /// - User is not authenticated (no Authorization header)
    /// - JWT validation failed (invalid or expired token)
    /// - Email claim is missing from JWT
    /// - User account was deleted from database but token still valid
    /// 
    /// **Performance Considerations:**
    /// - Single database query with eager loading
    /// - Email is indexed for fast lookup
    /// - Profile is included to avoid N+1 query issues
    /// - Consider adding AsNoTracking() if read-only access
    /// 
    /// **Example Usage:**
    /// <code>
    /// var currentUser = await _authHelper.GetCurrentUserAsync();
    /// if (currentUser == null)
    /// {
    ///     return Results.Unauthorized();
    /// }
    /// 
    /// // User is authenticated, access profile
    /// var profileId = currentUser.Profile.Id;
    /// var userName = currentUser.Profile.FirstName;
    /// </code>
    /// 
    /// **Best Practices:**
    /// - Always check for null before accessing user properties
    /// - Cache result in local variable if used multiple times in same method
    /// - Don't call repeatedly in loops - call once and reuse
    /// - Consider using in middleware for request-level caching
    /// 
    /// The method is safe to call from any authenticated endpoint and provides
    /// consistent user context across the application layer.
    /// </remarks>
    public async Task<User?> GetCurrentUserAsync()
    {
        var email = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        var user = await _dbContext.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == email);
        return user;
    }
}
