

namespace Api.Settings;

public class ConnectionStrings
{
    public const string Section = "ConnectionStrings";

    public string Postgresql { get; set; } = string.Empty;
}
