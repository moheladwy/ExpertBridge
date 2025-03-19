// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.Interfaces.Repositories;
using ExpertBridge.Api.Core.Interfaces.Services;

namespace ExpertBridge.Api.Application.Repositories.Users;

public sealed class UsersCacheRepository(
    UsersRepository repository,
    ICacheService cache) : IEntityRepository<Core.Entities.Users.User>
{
    public async Task<Core.Entities.Users.User?> GetByIdAsync(string id)
    {
        var key = $"User_{id}";

        var user = await cache.GetAsync<Core.Entities.Users.User>(key);
        if (user is not null) return user;

        user = await repository.GetByIdAsync(id);
        if (user is null) throw new UserNotFoundException("User not found");

        await cache.SetAsync(key, user);

        return user;
    }

    public async Task<Core.Entities.Users.User?> GetByIdAsNoTrackingAsync(string id)
    {
        var key = $"User_{id}";

        var user = await cache.GetAsync<Core.Entities.Users.User>(key);
        if (user is not null) return user;

        user = await repository.GetByIdAsNoTrackingAsync(id);
        if (user is null) throw new UserNotFoundException("User not found");

        await cache.SetAsync(key, user);

        return user;
    }

    public async Task<IEnumerable<Core.Entities.Users.User>> GetAllAsync() => await repository.GetAllAsync();

    public async Task<Core.Entities.Users.User?> GetFirstAsync(Expression<Func<Core.Entities.Users.User, bool>> predicate) =>
        await repository.GetFirstAsync(predicate);

    public async Task<Core.Entities.Users.User?> GetFirstAsNoTrackingAsync(Expression<Func<Core.Entities.Users.User, bool>> predicate) =>
        await repository.GetFirstAsNoTrackingAsync(predicate);

    public async Task AddAsync(Core.Entities.Users.User entity)
    {
        await repository.AddAsync(entity);
        await cache.SetAsync($"User_{entity.Id}", entity);
    }

    public async Task UpdateAsync(Core.Entities.Users.User entity)
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
