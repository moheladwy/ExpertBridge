using Amazon.Runtime;
using Amazon.S3;
using ExpertBridge.Application.Configurations;
using Microsoft.Extensions.Options;

namespace ExpertBridge.Api.Extensions;

internal static class ObjectStorage
{
    /// <summary>
    ///     Adds the S3 object service to the application builder.
    /// </summary>
    /// <param name="builder">
    ///     The WebApplicationBuilder to add the S3 object service to.
    /// </param>
    public static void AddS3ObjectService(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AwsConfigurations>(builder.Configuration.GetSection("AwsS3"));
        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var awsConfig = sp.GetRequiredService<IOptions<AwsConfigurations>>().Value;
            var credentials = new BasicAWSCredentials(awsConfig.Awskey, awsConfig.AwsSecret);
            var configurations = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsConfig.Region)
            };
            return new AmazonS3Client(credentials, configurations);
        });
    }
}
