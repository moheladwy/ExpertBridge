using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ExpertBridge.Extensions.AWS;

/// <summary>
///     Provides extension methods for integrating S3 object services into the application.
/// </summary>
internal static class S3
{
    /// <summary>
    ///     Adds the S3 object service to the application builder.
    /// </summary>
    /// <param name="builder">
    ///     The WebApplicationBuilder to add the S3 object service to.
    /// </param>
    public static void AddS3ObjectService(this WebApplicationBuilder builder) =>
        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var awsConfig = sp.GetRequiredService<IOptions<AwsSettings>>().Value;
            var credentials = new BasicAWSCredentials(awsConfig.AwsKey, awsConfig.AwsSecret);
            var configurations = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsConfig.Region)
            };
            return new AmazonS3Client(credentials, configurations);
        });
}
