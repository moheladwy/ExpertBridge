// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace ExpertBridge.Extensions.Caching;

/// <summary>
///     Provides extension methods for configuring FusionCache with Redis distributed caching in the ExpertBridge
///     application.
///     Sets up a hybrid two-level caching strategy combining in-memory and distributed caching layers.
/// </summary>
public static class FusionCaching
{
    /// <summary>
    ///     Registers and configures FusionCache as a hybrid cache with Redis distributed caching backend.
    ///     Sets up default entry options, JSON serialization, and background distributed cache operations for optimal
    ///     performance.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder to configure FusionCache for.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <remarks>
    ///     This method configures a two-level hybrid caching architecture:
    ///     **Level 1 (L1) - Memory Cache:**
    ///     - Fast in-process memory cache for immediate data access
    ///     - Reduces latency for frequently accessed data
    ///     - Volatile storage that doesn't survive application restarts
    ///     **Level 2 (L2) - Distributed Cache (Redis):**
    ///     - Shared cache across all application instances
    ///     - Persists data beyond application lifecycle
    ///     - Enables cache consistency in multi-instance deployments
    ///     **Configuration:**
    ///     - Default entry duration from CacheSettings.DefaultDurationInMinutes (default: 10 minutes)
    ///     - Background distributed cache operations enabled for non-blocking performance
    ///     - System.Text.Json serialization for efficient data transfer
    ///     - Redis instance name prefixing for key namespacing
    ///     **HybridCache Integration:**
    ///     Registered as IHybridCache for use with Microsoft.Extensions.Caching.Hybrid,
    ///     providing a standardized caching interface across the application for services like
    ///     profile caching, post caching, and job posting caching.
    /// </remarks>
    public static TBuilder AddFusionCache<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var cacheSettings = builder.Configuration.GetSection(CacheSettings.SectionName).Get<CacheSettings>()!;
        var redisConnectionString = builder.Configuration.GetConnectionString("Redis")!;
        builder.Services.AddFusionCache()
            .WithDefaultEntryOptions(options =>
            {
                options.Duration = TimeSpan.FromMinutes(cacheSettings.DefaultDurationInMinutes);
                // Setting this flag to true will execute most of the operations in the background.
                // Resulting in a performance boost.
                options.AllowBackgroundDistributedCacheOperations = true;
            })
            .WithSerializer(new FusionCacheSystemTextJsonSerializer())
            .WithDistributedCache(
#pragma warning disable CA2000
                new RedisCache(
                    new RedisCacheOptions
                    {
                        Configuration = redisConnectionString, InstanceName = cacheSettings.InstanceName
                    }
                )
#pragma warning restore CA2000
            ).AsHybridCache();
        return builder;
    }
}
