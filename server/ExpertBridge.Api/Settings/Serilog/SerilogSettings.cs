namespace ExpertBridge.Api.Settings.Serilog;

public class SerilogSettings
{
    public string[] Using { get; set; } = Array.Empty<string>();
    public MinimumLevel MinimumLevel { get; set; } = new();
    public WriteTo[] WriteTo { get; set; } = Array.Empty<WriteTo>();
    public string[] Enrich { get; set; } = Array.Empty<string>();
}
