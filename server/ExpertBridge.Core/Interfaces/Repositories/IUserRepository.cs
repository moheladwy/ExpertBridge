using ExpertBridge.Core.Entities.User;

namespace ExpertBridge.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsers();
    Task<User> GetUserById(string id);
    Task<User> GetUserByEmail(string email);
    Task<User> GetUserByUsername(string username);
    Task<User> GetUserByFirebaseId(string firebaseId);
    Task AddUser(User user);
    Task UpdateUser(User user);
    Task DeleteUser(string id);
}
