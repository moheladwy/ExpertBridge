using ExpertBridge.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Core.Entities.User;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Services;

public class UserService : IUserService
{
    public async Task<User> GetUserByFirebaseId(string firebaseId)
    {
        throw new NotImplementedException();
    }

    public async Task<User> RegisterNewUser(RegisterUserRequest request)
    {
        throw new NotImplementedException();
    }
}
