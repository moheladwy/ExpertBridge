using ExpertBridge.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Core.Entities.User;

namespace ExpertBridge.Core.Interfaces;

public interface IUserService
{
    Task<User> GetUserByFirebaseId(string firebaseId);
    Task<User> RegisterNewUser(RegisterUserRequest request);
}
