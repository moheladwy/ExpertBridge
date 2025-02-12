using System.Linq.Expressions;
using ExpertBridge.Core;
using ExpertBridge.Core.Entities.User;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Core.Interfaces.Services;
using ExpertBridge.Data.Repositories;

namespace ExpertBridge.Application.CachedRepositories;

public sealed class UserCacheRepository(
    UserRepository repository,
    ICacheService cache) : IEntityRepository<User>
{
    public async Task<User?> GetByIdAsync(string id)
    {
        var key = $"User_{id}";

        var user = await cache.GetAsync<User>(key);
        if (user is not null) return user;

        user = await repository.GetByIdAsync(id);
        if (user is null) throw new UserNotFoundException("User not found");

        await cache.SetAsync(key, user);

        return user;
    }

    public async Task<User?> GetByIdAsNoTrackingAsync(string id)
    {
        var key = $"User_{id}";

        var user = await cache.GetAsync<User>(key);
        if (user is not null) return user;

        user = await repository.GetByIdAsNoTrackingAsync(id);
        if (user is null) throw new UserNotFoundException("User not found");

        await cache.SetAsync(key, user);

        return user;
    }

    public async Task<IEnumerable<User>> GetAllAsync() => await repository.GetAllAsync();

    public async Task<User?> GetFirstAsync(Expression<Func<User, bool>> predicate) =>
        await repository.GetFirstAsync(predicate);

    public async Task<User?> GetFirstAsNoTrackingAsync(Expression<Func<User, bool>> predicate) =>
        await repository.GetFirstAsNoTrackingAsync(predicate);

    public async Task AddAsync(User entity)
    {
        await repository.AddAsync(entity);
        await cache.SetAsync($"User_{entity.Id}", entity);
    }

    public async Task UpdateAsync(User entity)
    {
        await repository.UpdateAsync(entity);
        await cache.UpdateAsync($"User_{entity.Id}", entity);
    }

    public async Task DeleteAsync(string id)
    {
        await repository.DeleteAsync(id);
        await cache.RemoveAsync($"User_{id}");
    }
}
