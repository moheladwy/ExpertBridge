// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Settings;

public sealed class AwsSettings
{
    public const string Section = "AwsS3";

    public string Region { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string AwsKey { get; set; } = string.Empty;
    public string AwsSecret { get; set; } = string.Empty;
    public string BucketUrl { get; set; } = string.Empty;
    public long MaxFileSize { get; set; }
    public string CacheControl { get; set; } = string.Empty;
}
