using System.Linq.Expressions;
using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.Interfaces.Repositories;
using ExpertBridge.Api.Core.Interfaces.Services;
using UserProfile = ExpertBridge.Api.Core.Entities.Profile.Profile;
namespace ExpertBridge.Api.Application.Repositories.Profile;

public class ProfileCacheRepository(
    ICacheService cacheService,
    ProfileRepository profileRepository
    ) : IEntityRepository<Core.Entities.Profile.Profile>
{
    public async Task<UserProfile?> GetByIdAsync(string id)
    {
        // The key of the profile in the cache.
        var key = $"Profile_{id}";

        // Try to get the profile from the cache.
        var profile = await cacheService.GetAsync<UserProfile>(key);
        if (profile is not null) return profile;

        // If the profile is not in the cache, get it from the repository.
        profile = await profileRepository.GetByIdAsync(id);
        if (profile is null) throw new ProfileNotFoundException("Profile not found");

        // Set the profile in the cache.
        await cacheService.SetAsync(key, profile);

        return profile;
    }

    public async Task<UserProfile?> GetByIdAsNoTrackingAsync(string id)
    {
        // The key of the profile in the cache.
        var key = $"Profile_{id}";

        // Try to get the profile from the cache.
        var profile = await cacheService.GetAsync<UserProfile>(key);
        if (profile is not null) return profile;

        // If the profile is not in the cache, get it from the repository.
        profile = await profileRepository.GetByIdAsNoTrackingAsync(id);
        if (profile is null) throw new ProfileNotFoundException("Profile not found");

        // Set the profile in the cache.
        await cacheService.SetAsync(key, profile);

        return profile;
    }

    public async Task<IEnumerable<UserProfile>> GetAllAsync() => await profileRepository.GetAllAsync();

    public async Task<UserProfile?> GetFirstAsync(Expression<Func<UserProfile, bool>> predicate)
    {
        var profile = await profileRepository.GetFirstAsync(predicate);
        if (profile is null) throw new ProfileNotFoundException("Profile not found");

        var key = $"Profile_{profile.Id}";
        await cacheService.SetAsync(key, profile);

        return profile;
    }

    public async Task<UserProfile?> GetFirstAsNoTrackingAsync(Expression<Func<UserProfile, bool>> predicate)
    {
        var profile = await profileRepository.GetFirstAsNoTrackingAsync(predicate);
        if (profile is null) throw new ProfileNotFoundException("Profile not found");

        var key = $"Profile_{profile.Id}";
        await cacheService.SetAsync(key, profile);

        return profile;
    }

    public async Task AddAsync(UserProfile entity)
    {
        await profileRepository.AddAsync(entity);
        await cacheService.SetAsync($"Profile_{entity.Id}", entity);
    }

    public async Task UpdateAsync(UserProfile entity)
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
