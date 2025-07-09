using ExpertBridge.Application.Helpers;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Data.DatabaseContexts;

namespace ExpertBridge.Application.DomainServices
{
    public class UserService
    {
        private readonly ExpertBridgeDbContext _dbContext;
        private readonly AuthorizationHelper _authHelper; // Keep if GetCurrentUserAsync logic is complex

        public UserService(
            ExpertBridgeDbContext dbContext,
            AuthorizationHelper authHelper)
        {
            _dbContext = dbContext;
            _authHelper = authHelper;
        }

        public async Task<User?> GetCurrentUserPopulatedModelAsync()
        {
            return await _authHelper.GetCurrentUserAsync();
        }
        public async Task<string> GetCurrentUserProfileIdOrEmptyAsync()
        {
            var user = await _authHelper.GetCurrentUserAsync();
            return user?.Profile?.Id ?? string.Empty;
        }

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
}
