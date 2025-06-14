// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Settings.Serilog;

public class SerilogSettings
{
    public const string Section = "Serilog";

    public string[] Using { get; set; } = Array.Empty<string>();
    public MinimumLevel MinimumLevel { get; set; } = new();
    public WriteTo[] WriteTo { get; set; } = Array.Empty<WriteTo>();
    public string[] Enrich { get; set; } = Array.Empty<string>();
}
