// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace ExpertBridge.Extensions.Caching;

/// <summary>
///     Provides extension methods for configuring FusionCache in a .NET application using Microsoft.Extensions.Hosting.
/// </summary>
public static class FusionCaching
{
    /// <summary>
    ///     Configures FusionCache in a .NET application by setting up default caching options, serialization,
    ///     and distributed caching using Redis, based on the application's configuration.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder to which FusionCache will be added and configured.</param>
    /// <returns>Returns the given host application builder with FusionCache configured.</returns>
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
                        Configuration = redisConnectionString,
                        InstanceName = cacheSettings.InstanceName
                    }
                )
#pragma warning restore CA2000
            ).AsHybridCache();
        return builder;
    }
}
