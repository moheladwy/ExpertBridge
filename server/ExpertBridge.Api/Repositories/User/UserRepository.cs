using System.Linq.Expressions;
using ExpertBridge.Core;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Repositories.User;

public class UserRepository(ExpertBridgeDbContext db) : IEntityRepository<Core.Entities.User.User>
{
    public async Task<Core.Entities.User.User?> GetByIdAsNoTrackingAsync(string id) =>
        await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Core.Entities.User.User?> GetByIdAsync(string id) => await db.Users.FindAsync(id);

    public async Task<IEnumerable<Core.Entities.User.User>> GetAllAsync() => await db.Users.AsNoTracking().ToListAsync();

    public async Task<Core.Entities.User.User?> GetFirstAsNoTrackingAsync(Expression<Func<Core.Entities.User.User, bool>> predicate) =>
        await db.Users.AsNoTracking().FirstOrDefaultAsync(predicate);

    public async Task<Core.Entities.User.User?> GetFirstAsync(Expression<Func<Core.Entities.User.User, bool>> predicate) =>
        await db.Users.FirstOrDefaultAsync(predicate);

    public async Task AddAsync(Core.Entities.User.User entity)
    {
        await db.Users.AddAsync(entity);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Core.Entities.User.User entity)
    {
        db.Users.Update(entity);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var user = await GetByIdAsync(id);
        if (user is null) throw new UserNotFoundException("User not found");
        user.IsDeleted = true;
        await db.SaveChangesAsync();
    }
}
