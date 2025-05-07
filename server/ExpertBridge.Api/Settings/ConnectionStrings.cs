

namespace ExpertBridge.Api.Settings;

public sealed class ConnectionStrings
{
    public const string Section = "ConnectionStrings";

    public string Postgresql { get; set; } = string.Empty;
}
