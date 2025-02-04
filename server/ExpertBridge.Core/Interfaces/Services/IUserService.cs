using ExpertBridge.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Core.DTOs.Responses;
using ExpertBridge.Core.Entities.User;

namespace ExpertBridge.Core.Interfaces.Services;

public interface IUserService
{
    Task<UserResponse> GetUserByFirebaseId(string firebaseId);
    Task<UserResponse> RegisterNewUser(RegisterUserRequest request);
}
