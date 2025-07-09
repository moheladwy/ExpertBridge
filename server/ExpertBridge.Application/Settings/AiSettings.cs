namespace ExpertBridge.Application.Settings;

public sealed class AiSettings
{
    public const string Section = "AI";

    public string PostCategorizationUrl { get; set; } = string.Empty;
}
