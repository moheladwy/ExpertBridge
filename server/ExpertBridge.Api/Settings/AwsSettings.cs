// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace ExpertBridge.Api.Settings;

public class AwsSettings
{
    public string Region { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string AwsKey { get; set; } = string.Empty;
    public string AwsSecret { get; set; } = string.Empty;
    public string BucketUrl { get; set; } = string.Empty;
}
