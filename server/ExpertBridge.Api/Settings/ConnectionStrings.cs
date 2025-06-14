// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Settings;

public sealed class ConnectionStrings
{
    public const string Section = "ConnectionStrings";

    public string Postgresql { get; set; } = string.Empty;

    public string Redis { get; set; } = string.Empty;
}
