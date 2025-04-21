namespace ExpertBridge.Api.Settings.Serilog;

public class WriteTo
{
    public string Name { get; set; } = string.Empty;
    public Args Args { get; set; } = new();
}
