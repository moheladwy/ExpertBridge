// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.Interfaces.Repositories;
using ExpertBridge.Api.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Application.Repositories.Users;

public class UsersRepository(ExpertBridgeDbContext db) : IEntityRepository<Core.Entities.Users.User>
{
    public async Task<Core.Entities.Users.User?> GetByIdAsNoTrackingAsync(string id) =>
        await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Core.Entities.Users.User?> GetByIdAsync(string id) => await db.Users.FindAsync(id);

    public async Task<IEnumerable<Core.Entities.Users.User>> GetAllAsync() => await db.Users.AsNoTracking().ToListAsync();

    public async Task<Core.Entities.Users.User?> GetFirstAsNoTrackingAsync(Expression<Func<Core.Entities.Users.User, bool>> predicate) =>
        await db.Users.AsNoTracking().FirstOrDefaultAsync(predicate);

    public async Task<Core.Entities.Users.User?> GetFirstAsync(Expression<Func<Core.Entities.Users.User, bool>> predicate) =>
        await db.Users.FirstOrDefaultAsync(predicate);

    public async Task AddAsync(Core.Entities.Users.User entity)
    {
        await db.Users.AddAsync(entity);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Core.Entities.Users.User entity)
    {
        db.Users.Update(entity);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var user = await GetByIdAsync(id);
        if (user is null) throw new UserNotFoundException("User not found");
        user.IsDeleted = true;
        await UpdateAsync(user);
    }
}
