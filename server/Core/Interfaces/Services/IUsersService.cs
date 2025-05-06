using Core.Requests.RegisterUser;
using Core.Requests.UpdateUserRequest;
using Core.Responses;

namespace Core.Interfaces.Services;

public interface IUsersService
{
    Task<UserResponse> GetUserByIdentityProviderId(string identityProviderId);
    Task<UserResponse> GetUserByEmailAsync(string email);
    Task<UserResponse> RegisterNewUser(RegisterUserRequest request);
    Task<UserResponse> UpdateUserAsync(UpdateUserRequest request);
    Task DeleteUserAsync(string identityProviderId);
    Task<bool> IsUserBannedAsync(string identityProviderId);
    Task<bool> IsUserVerifiedAsync(string email);
    Task<bool> IsUserDeletedAsync(string identityProviderId);
    Task BanUserAsync(string identityProviderId);
    Task VerifyUserAsync(string email);
}
