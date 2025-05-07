// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Settings;

public sealed class PostCategorizerSettings
{
    public const string Section = "PostCategorizer";

    public string BaseUrl { get; set; } = string.Empty;
}
