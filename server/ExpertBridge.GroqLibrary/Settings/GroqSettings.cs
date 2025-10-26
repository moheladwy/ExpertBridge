// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.GroqLibrary.Settings;

public class GroqSettings
{
    public const string Section = "Groq";
    public string ApiKey { get; set; }
    public string Model { get; set; }
}
