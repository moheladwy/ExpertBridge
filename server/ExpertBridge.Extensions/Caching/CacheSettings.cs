// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Extensions.Caching;

/// <summary>
/// Represents the configuration settings for distributed caching using Redis in the ExpertBridge application.
/// Defines cache instance name and default expiration duration for FusionCache hybrid caching strategy.
/// </summary>
/// <remarks>
/// These settings are loaded from the "Redis" configuration section and control:
/// - Redis instance name prefix for cache key namespacing
/// - Default cache entry expiration time in minutes
/// 
/// Used by FusionCache to configure a two-level caching strategy combining:
/// - Level 1 (L1): In-memory cache for fast local access
/// - Level 2 (L2): Distributed Redis cache for cross-instance consistency
/// </remarks>
public class CacheSettings
{
    /// <summary>
    /// Gets the configuration section name for Redis cache settings.
    /// </summary>
    public const string SectionName = "Redis";

    /// <summary>
    /// Gets or sets the Redis instance name used as a prefix for all cache keys to avoid collisions in shared Redis instances.
    /// </summary>
    public string InstanceName { get; set; }

    /// <summary>
    /// Gets or sets the default duration in minutes that cache entries remain valid before expiration.
    /// Defaults to 10 minutes if not specified in configuration.
    /// </summary>
    public int DefaultDurationInMinutes { get; set; } = 10;
}
