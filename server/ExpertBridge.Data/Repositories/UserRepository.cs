using ExpertBridge.Core;
using ExpertBridge.Core.Entities.User;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Data.Repositories;

public class UserRepository(ExpertBridgeDbContext db) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllUsers() => await db.Users.AsNoTracking().ToListAsync();

    public async Task<User> GetUserById(string id)
    {
        var user = await db.Users.FindAsync(id);
        return user ?? throw new UserNotFoundException("User not found");
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        return user ?? throw new UserNotFoundException("User not found");
    }

    public async Task<User> GetUserByUsername(string username)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
        return user ?? throw new UserNotFoundException("User not found");
    }

    public async Task<User> GetUserByFirebaseId(string firebaseId)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.FirebaseId == firebaseId);
        return user ?? throw new UserNotFoundException("User not found");
    }

    public async Task AddUser(User user)
    {
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();
    }

    public async Task UpdateUser(User user)
    {
        db.Users.Update(user);
        await db.SaveChangesAsync();
    }

    public async Task DeleteUser(string id)
    {
        var user = await GetUserById(id);
        user.isDeleted = true;
        db.Users.Update(user);
        await db.SaveChangesAsync();
    }
}
