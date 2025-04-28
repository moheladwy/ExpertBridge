

using System.Security.Claims;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Helpers
{
    public class AuthorizationHelper
    {
        private readonly ExpertBridgeDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationHelper(
            ExpertBridgeDbContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the currently signed in user with Profile nav prop populated.
        /// </summary>
        /// <param name="claims">The User prop from the controller.</param>
        /// <returns>
        /// 1. The user model from the database populated with Profile nav prop. <br/>
        /// 2. null if no authentication took place.
        /// </returns>
        public async Task<User?> GetCurrentUserAsync()
        {
            string? email = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

            var user = await _dbContext.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }
    }
}
