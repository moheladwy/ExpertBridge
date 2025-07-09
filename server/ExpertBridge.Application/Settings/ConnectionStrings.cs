namespace ExpertBridge.Application.Settings;

public sealed class ConnectionStrings
{
    public const string Section = "ConnectionStrings";

    public string Postgresql { get; set; } = string.Empty;

    public string Redis { get; set; } = string.Empty;
}
