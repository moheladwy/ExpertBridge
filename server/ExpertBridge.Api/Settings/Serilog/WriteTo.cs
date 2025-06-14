// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Settings.Serilog;

public class WriteTo
{
    public string Name { get; set; } = string.Empty;
    public Args Args { get; set; } = new();
}
