using System.Linq.Expressions;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using UserProfile = ExpertBridge.Core.Entities.Profile.Profile;

namespace ExpertBridge.Api.Repositories.Profile;

public class ProfileRepository(
    ExpertBridgeDbContext db,
    IEntityRepository<Core.Entities.User.User> userRepository
    ) : IEntityRepository<UserProfile>
{
    public async Task<UserProfile?> GetByIdAsync(string id) =>
        await db.Profiles.FindAsync(id);

    public async Task<UserProfile?> GetByIdAsNoTrackingAsync(string id) =>
        await db.Profiles.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<UserProfile>> GetAllAsync() =>
        await db.Profiles.AsNoTracking().ToListAsync();

    public async Task<UserProfile?> GetFirstAsync(Expression<Func<UserProfile, bool>> predicate) =>
        await db.Profiles.FirstOrDefaultAsync(predicate);

    public async Task<UserProfile?> GetFirstAsNoTrackingAsync(Expression<Func<UserProfile, bool>> predicate) =>
        await db.Profiles.AsNoTracking().FirstOrDefaultAsync(predicate);

    public async Task AddAsync(UserProfile entity)
    {
        await db.Profiles.AddAsync(entity);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserProfile entity)
    {
        db.Profiles.Update(entity);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var profile = await db.Profiles.FindAsync(id);
        if (profile is not null) await userRepository.DeleteAsync(profile.UserId);
    }
}
