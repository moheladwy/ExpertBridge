using System.Text.Json;
using Core.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Api.Services;

/// <summary>
///     Represents a caching service implementation that provides methods
///     for storing, retrieving, updating, and removing values in a distributed cache.
///     Implements the <see cref="ICacheService" /> interface.
/// </summary>
public sealed class CacheService(IDistributedCache cache) : ICacheService
{
    /// <summary>
    ///     Provides configuration options for entries stored in the distributed cache.
    /// </summary>
    /// <remarks>
    ///     Defines properties for cache entry behavior, such as the time period
    ///     an entry remains in the cache before it is automatically removed.
    /// </remarks>
    private static readonly DistributedCacheEntryOptions s_options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    /// <summary>
    ///     Asynchronously retrieves a value of type <typeparamref name="T" /> from the cache using the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve from the cache.</typeparam>
    /// <param name="key">The key associated with the cached value to retrieve.</param>
    /// <returns>
    ///     The deserialized value of type <typeparamref name="T" /> from the cache if the key exists,
    ///     or <c>null</c> if the key is not found in the cache or the value is not deserializable.
    /// </returns>
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await cache.GetStringAsync(key);
        return value is null ? default : JsonSerializer.Deserialize<T>(value);
    }

    /// <summary>
    ///     Asynchronously stores an entity of type <typeparamref name="T" /> in the cache using the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the entity to store in the cache.</typeparam>
    /// <param name="key">The key associated with the cached value.</param>
    /// <param name="entity">The entity to serialize and store in the cache.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetAsync<T>(string key, T entity) =>
        await cache.SetStringAsync(key, JsonSerializer.Serialize(entity), s_options);

    /// <summary>
    ///     Asynchronously updates an existing value of type <typeparamref name="T" /> in the cache
    ///     by removing the old entry and setting a new entry with the given key and value.
    /// </summary>
    /// <typeparam name="T">The type of the value to update in the cache.</typeparam>
    /// <param name="key">The key associated with the cache entry to update.</param>
    /// <param name="entity">The new value of type <typeparamref name="T" /> to associate with the given key.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    public async Task UpdateAsync<T>(string key, T entity)
    {
        await RemoveAsync(key);
        await SetAsync(key, entity);
    }

    /// <summary>
    ///     Asynchronously removes a cached value associated with the specified key in the distributed cache.
    /// </summary>
    /// <param name="key">The key of the cached value to be removed.</param>
    /// <returns>A task that represents the asynchronous remove operation.</returns>
    public async Task RemoveAsync(string key) => await cache.RemoveAsync(key);
}
