using System.Linq.Expressions;
using ExpertBridge.Core;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Core.Interfaces.Services;

namespace ExpertBridge.Application.Repositories.User;

public sealed class UserCacheRepository(
    UserRepository repository,
    ICacheService cache) : IEntityRepository<Core.Entities.User.User>
{
    public async Task<Core.Entities.User.User?> GetByIdAsync(string id)
    {
        var key = $"User_{id}";

        var user = await cache.GetAsync<Core.Entities.User.User>(key);
        if (user is not null) return user;

        user = await repository.GetByIdAsync(id);
        if (user is null) throw new UserNotFoundException("User not found");

        await cache.SetAsync(key, user);

        return user;
    }

    public async Task<Core.Entities.User.User?> GetByIdAsNoTrackingAsync(string id)
    {
        var key = $"User_{id}";

        var user = await cache.GetAsync<Core.Entities.User.User>(key);
        if (user is not null) return user;

        user = await repository.GetByIdAsNoTrackingAsync(id);
        if (user is null) throw new UserNotFoundException("User not found");

        await cache.SetAsync(key, user);

        return user;
    }

    public async Task<IEnumerable<Core.Entities.User.User>> GetAllAsync() => await repository.GetAllAsync();

    public async Task<Core.Entities.User.User?> GetFirstAsync(Expression<Func<Core.Entities.User.User, bool>> predicate) =>
        await repository.GetFirstAsync(predicate);

    public async Task<Core.Entities.User.User?> GetFirstAsNoTrackingAsync(Expression<Func<Core.Entities.User.User, bool>> predicate) =>
        await repository.GetFirstAsNoTrackingAsync(predicate);

    public async Task AddAsync(Core.Entities.User.User entity)
    {
        await repository.AddAsync(entity);
        await cache.SetAsync($"User_{entity.Id}", entity);
    }

    public async Task UpdateAsync(Core.Entities.User.User entity)
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
