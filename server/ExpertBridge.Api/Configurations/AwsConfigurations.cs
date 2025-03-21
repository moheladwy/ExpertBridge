// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace ExpertBridge.Api.Configurations;

public class AwsConfigurations
{
    [JsonPropertyName("AwsKey")]
    public string Awskey { get; set; }

    [JsonPropertyName("AwsSecret")]
    public string AwsSecret { get; set; }

    [JsonPropertyName("BucketName")]
    public string BucketName { get; set; }

    [JsonPropertyName("Region")]
    public string Region { get; set; }

    [JsonPropertyName("BucketUrl")]
    public string BucketUrl { get; set; }
}
