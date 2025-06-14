// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Extensions.Caching;

/// <summary>
///     Represents the configuration settings required for caching using Redis.
/// </summary>
public class CacheSettings
{
    public const string SectionName = "Redis";

    public string InstanceName { get; set; }
    public int DefaultDurationInMinutes { get; set; } = 10;
}
