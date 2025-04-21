namespace ExpertBridge.Api.Settings.Serilog;

public class MinimumLevel
{
    public string Default { get; set; } = string.Empty;
    public Dictionary<string, string> Override { get; set; } = new();
}
