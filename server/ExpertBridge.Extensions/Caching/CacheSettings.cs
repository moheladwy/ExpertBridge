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
