// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.GroqLibrary.Models;

public class Tool
{
    public string Type { get; set; } = "function";
    public Function Function { get; set; }
}
