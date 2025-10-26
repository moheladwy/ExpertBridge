// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Nodes;

namespace ExpertBridge.GroqLibrary.Models;

public class Function
{
    public string Name { get; set; }
    public string Description { get; set; }
    public JsonObject Parameters { get; set; }
    public Func<string, Task<string>> ExecuteAsync { get; set; }
}
