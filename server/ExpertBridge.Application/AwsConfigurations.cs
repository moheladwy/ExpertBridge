using System.Text.Json.Serialization;

namespace ExpertBridge.Application;

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
