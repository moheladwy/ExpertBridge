using System.Security.Claims;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Application.Helpers;

/// <summary>
///     Provides helper methods for authorization and user context management in API requests.
/// </summary>
/// <remarks>
///     This helper class simplifies accessing the currently authenticated user from the HTTP context
///     throughout the application layer, particularly in domain services that need user context.
///     <code>
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
///     Should be registered as scoped service to match HTTP request lifetime.
/// </remarks>
public class AuthorizationHelper
{
    /// <summary>
    ///     The database context for querying user entities.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     The HTTP context accessor for retrieving authentication claims from the current request.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AuthorizationHelper" /> class with database context and HTTP context
    ///     accessor.
    /// </summary>
    /// <param name="dbContext">
    ///     The database context for loading user entities.
    /// </param>
    /// <param name="httpContextAccessor">
    ///     The HTTP context accessor for accessing authentication claims from the current request.
    /// </param>
    /// <remarks>
    ///     Both dependencies are injected as scoped services, ensuring they're tied to the HTTP request lifetime.
    ///     IHttpContextAccessor must be registered in DI: services.AddHttpContextAccessor()
    /// </remarks>
    public AuthorizationHelper(
        ExpertBridgeDbContext dbContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///     Retrieves the currently authenticated user from the database with the Profile navigation property populated.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous operation, containing:
    ///     - The <see cref="User" /> entity with Profile loaded if the user is authenticated
    ///     - null if no user is authenticated or authentication validation failed
    /// </returns>
    public async Task<User?> GetCurrentUserAsync()
    {
        var email = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        var user = await _dbContext.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == email);
        return user;
    }
}
