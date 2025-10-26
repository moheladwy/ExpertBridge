// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ExpertBridge.Extensions.AWS;

/// <summary>
/// Provides extension methods for integrating Amazon S3 object storage services into the ExpertBridge application.
/// Configures AWS SDK client with credentials and regional settings for media file operations.
/// </summary>
public static class S3
{
    /// <summary>
    /// Registers the Amazon S3 client as a singleton service in the dependency injection container.
    /// Configures the S3 client with AWS credentials and region from <see cref="AwsSettings"/> for media storage operations.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder to configure S3 services for.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <remarks>
    /// This method:
    /// - Retrieves AWS credentials (access key and secret) from AwsSettings configuration
    /// - Creates BasicAWSCredentials for authentication
    /// - Configures the AmazonS3Client with the specified region endpoint
    /// - Registers IAmazonS3 as a singleton for use in media attachment services
    ///
    /// The configured S3 client is used throughout the application for:
    /// - Generating presigned URLs for client-side uploads (PostMedia, ProfileMedia, etc.)
    /// - Managing media file lifecycle and access control
    /// - Storing attachments in the expert-bridge-media bucket
    /// </remarks>
    public static TBuilder AddS3ObjectService<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
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

        return builder;
    }
}
