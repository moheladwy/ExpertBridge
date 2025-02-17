using System.Linq.Expressions;
using ExpertBridge.Core;
using ExpertBridge.Core.Entities.User;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Data.Repositories;

public class UserRepository(ExpertBridgeDbContext db) : IEntityRepository<User>
{
    public async Task<User?> GetByIdAsNoTrackingAsync(string id) =>
        await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<User?> GetByIdAsync(string id) => await db.Users.FindAsync(id);

    public async Task<IEnumerable<User>> GetAllAsync() => await db.Users.AsNoTracking().ToListAsync();

    public async Task<User?> GetFirstAsNoTrackingAsync(Expression<Func<User, bool>> predicate) =>
        await db.Users.AsNoTracking().FirstOrDefaultAsync(predicate);

    public async Task<User?> GetFirstAsync(Expression<Func<User, bool>> predicate) =>
        await db.Users.FirstOrDefaultAsync(predicate);

    public async Task AddAsync(User entity)
    {
        await db.Users.AddAsync(entity);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(User entity)
    {
        db.Users.Update(entity);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var user = await GetByIdAsync(id);
        if (user is null) throw new UserNotFoundException("User not found");
        user.isDeleted = true;
        await db.SaveChangesAsync();
    }
}
