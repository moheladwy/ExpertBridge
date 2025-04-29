// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Api.Application.Interfaces.Repositories;
using ExpertBridge.Api.Application.Interfaces.Services;
using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.Entities.Profiles;
using ExpertBridge.Api.Models;
namespace ExpertBridge.Api.Application.Repositories.Profiles;

public class ProfilesCacheRepository(
    ICacheService cacheService,
    ProfilesRepository profileRepository
    ) : IEntityRepository<Profile>
{
    public async Task<Profile?> GetByIdAsync(string id)
    {
        // The key of the profile in the cache.
        var key = $"Profile_{id}";

        // Try to get the profile from the cache.
        var profile = await cacheService.GetAsync<Profile>(key);
        if (profile is not null) return profile;

        // If the profile is not in the cache, get it from the repository.
        profile = await profileRepository.GetByIdAsync(id);
        if (profile is null) throw new ProfileNotFoundException("Profile not found");

        // Set the profile in the cache.
        await cacheService.SetAsync(key, profile);

        return profile;
    }

    public async Task<Profile?> GetByIdAsNoTrackingAsync(string id)
    {
        // The key of the profile in the cache.
        var key = $"Profile_{id}";

        // Try to get the profile from the cache.
        var profile = await cacheService.GetAsync<Profile>(key);
        if (profile is not null) return profile;

        // If the profile is not in the cache, get it from the repository.
        profile = await profileRepository.GetByIdAsNoTrackingAsync(id);
        if (profile is null) throw new ProfileNotFoundException("Profile not found");

        // Set the profile in the cache.
        await cacheService.SetAsync(key, profile);

        return profile;
    }

    public async Task<IEnumerable<Profile>> GetAllAsync() => await profileRepository.GetAllAsync();

    public async Task<Profile?> GetFirstAsync(Expression<Func<Profile, bool>> predicate)
    {
        var profile = await profileRepository.GetFirstAsync(predicate);
        if (profile is null) throw new ProfileNotFoundException("Profile not found");

        var key = $"Profile_{profile.Id}";
        await cacheService.SetAsync(key, profile);

        return profile;
    }

    public async Task<Profile?> GetFirstAsNoTrackingAsync(Expression<Func<Profile, bool>> predicate)
    {
        var profile = await profileRepository.GetFirstAsNoTrackingAsync(predicate);
        if (profile is null) throw new ProfileNotFoundException("Profile not found");

        var key = $"Profile_{profile.Id}";
        await cacheService.SetAsync(key, profile);

        return profile;
    }

    public async Task AddAsync(Profile entity)
    {
        await profileRepository.AddAsync(entity);
        await cacheService.SetAsync($"Profile_{entity.Id}", entity);
    }

    public async Task UpdateAsync(Profile entity)
    {
        await profileRepository.UpdateAsync(entity);
        await cacheService.UpdateAsync($"Profile_{entity.Id}", entity);
    }

    public async Task DeleteAsync(string id)
    {
        await profileRepository.DeleteAsync(id);
        await cacheService.RemoveAsync($"Profile_{id}");
    }
}
