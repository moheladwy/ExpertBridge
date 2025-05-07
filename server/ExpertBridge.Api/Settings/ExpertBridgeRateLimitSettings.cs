namespace ExpertBridge.Api.Settings;

public sealed class ExpertBridgeRateLimitSettings
{
    public const string SectionName = "RateLimit";

    public int PermitLimit { get; set; }
    public int Window { get; set; }
    public int QueueLimit { get; set; }
}
