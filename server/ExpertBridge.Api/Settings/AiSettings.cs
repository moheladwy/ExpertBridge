// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Settings;

public sealed class AiSettings
{
    public const string Section = "AI";

    public string PostCategorizationUrl { get; set; } = string.Empty;
}
