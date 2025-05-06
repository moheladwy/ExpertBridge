

namespace Api.Settings
{
    public class ExpertBridgeRateLimitSettings
    {
        public const string SectionName = "RateLimit";

        public int PermitLimit { get; set; }
        public int Window { get; set; }
        public int QueueLimit { get; set; }
    }
}
