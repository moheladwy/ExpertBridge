using System.Linq.Expressions;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Core.Interfaces.Services;
using UserProfile = ExpertBridge.Core.Entities.Profile.Profile;
namespace ExpertBridge.Api.Repositories.Profile;

public class ProfileCacheRepository(
    ICacheService cacheService,
    ProfileRepository profileRepository
    ) : IEntityRepository<UserProfile>
{
    public Task<UserProfile?> GetByIdAsync(string id) => throw new NotImplementedException();

    public Task<UserProfile?> GetByIdAsNoTrackingAsync(string id) => throw new NotImplementedException();

    public Task<IEnumerable<UserProfile>> GetAllAsync() => throw new NotImplementedException();

    public Task<UserProfile?> GetFirstAsync(Expression<Func<UserProfile, bool>> predicate) => throw new NotImplementedException();

    public Task<UserProfile?> GetFirstAsNoTrackingAsync(Expression<Func<UserProfile, bool>> predicate) => throw new NotImplementedException();

    public Task AddAsync(UserProfile entity) => throw new NotImplementedException();

    public Task UpdateAsync(UserProfile entity) => throw new NotImplementedException();

    public Task DeleteAsync(string id) => throw new NotImplementedException();
}
