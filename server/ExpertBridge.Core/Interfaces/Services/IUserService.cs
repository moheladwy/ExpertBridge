using ExpertBridge.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Core.DTOs.Responses;
using ExpertBridge.Core.Entities.User;

namespace ExpertBridge.Core.Interfaces.Services;

public interface IUserService
{
    Task<UserResponse> GetUserByIdentityProviderId(string firebaseId);
    Task<UserResponse> RegisterNewUser(RegisterUserRequest requestDto);
}
