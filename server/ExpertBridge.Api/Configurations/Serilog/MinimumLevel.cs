namespace ExpertBridge.Api.Configurations.Serilog;

public class MinimumLevel
{
    public string Default { get; set; } = string.Empty;
    public Dictionary<string, string> Override { get; set; } = new();
}
