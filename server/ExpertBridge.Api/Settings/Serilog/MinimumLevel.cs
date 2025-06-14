// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Settings.Serilog;

public class MinimumLevel
{
    public string Default { get; set; } = string.Empty;
    public Dictionary<string, string> Override { get; set; } = new();
}
