// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Settings;

public sealed class FirebaseAuthSettings
{
    public const string Section = "Authentication:Firebase";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string TokenUri { get; set; } = string.Empty;
}
