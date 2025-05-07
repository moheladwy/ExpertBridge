namespace ExpertBridge.Api.Settings;

public sealed class AiSettings
{
    public const string Section = "AI";

    public string PostCategorizationUrl { get; set; } = string.Empty;
}
