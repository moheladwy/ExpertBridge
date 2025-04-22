// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.Entities.Profiles;
using ExpertBridge.Api.Core.Entities.Users;
using ExpertBridge.Api.Core.Interfaces.Repositories;
using ExpertBridge.Api.Data.DatabaseContexts;
using ExpertBridge.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Application.Repositories.Profiles;

public class ProfilesRepository(
    ExpertBridgeDbContext db,
    IEntityRepository<User> userRepository
    ) : IEntityRepository<Profile>
{
    public async Task<Profile?> GetByIdAsync(string id) =>
        await db.Profiles.FindAsync(id);

    public async Task<Profile?> GetByIdAsNoTrackingAsync(string id) =>
        await db.Profiles.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Profile>> GetAllAsync() =>
        await db.Profiles.AsNoTracking().ToListAsync();

    public async Task<Profile?> GetFirstAsync(Expression<Func<Profile, bool>> predicate) =>
        await db.Profiles.FirstOrDefaultAsync(predicate);

    public async Task<Profile?> GetFirstAsNoTrackingAsync(Expression<Func<Profile, bool>> predicate) =>
        await db.Profiles.AsNoTracking().FirstOrDefaultAsync(predicate);

    public async Task AddAsync(Profile entity)
    {
        await db.Profiles.AddAsync(entity);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Profile entity)
    {
        db.Profiles.Update(entity);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var profile = await db.Profiles.FindAsync(id) ??
                      throw new ProfileNotFoundException("Profile not found");
        await userRepository.DeleteAsync(profile.UserId);
    }
}
