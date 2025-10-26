// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Host.Resources;

internal static class Redis
{
    public static IResourceBuilder<RedisResource> GetRedisResource(this IDistributedApplicationBuilder builder)
    {
        var redis = builder
            .AddRedis("Redis", 6379)
            .WithImage("redis", "alpine")
            .WithContainerName("expertbridge-redis")
            .WithDataVolume("expertbridge-redis-data")
            .WithPersistence(TimeSpan.FromMinutes(5))
            .WithLifetime(ContainerLifetime.Persistent)
            .WithOtlpExporter()
            .WithExternalHttpEndpoints();

        return redis;
    }
}
