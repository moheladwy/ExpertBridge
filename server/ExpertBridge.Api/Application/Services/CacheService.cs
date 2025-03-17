using System.Text.Json;
using ExpertBridge.Api.Core.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace ExpertBridge.Api.Application.Services;

public class CacheService(IDistributedCache cache) : ICacheService
{
    private static readonly DistributedCacheEntryOptions s_options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await cache.GetStringAsync(key);
        return value is null ? default : JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetAsync<T>(string key, T entity) =>
        await cache.SetStringAsync(key, JsonSerializer.Serialize(entity), s_options);

    public async Task UpdateAsync<T>(string key, T entity)
    {
        await RemoveAsync(key);
        await SetAsync(key, entity);
    }

    public async Task RemoveAsync(string key) => await cache.RemoveAsync(key);
}
