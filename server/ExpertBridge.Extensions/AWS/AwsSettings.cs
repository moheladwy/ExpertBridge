// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Extensions.AWS;

/// <summary>
/// Represents configuration settings for AWS S3 integration in the ExpertBridge application.
/// Contains credentials, bucket information, and file upload constraints for the expert-bridge-media S3 bucket.
/// </summary>
/// <remarks>
/// These settings are loaded from the "AwsS3" configuration section and include:
/// - AWS credentials (access key and secret) for S3 operations
/// - Region and bucket name for media storage location
/// - File size limits and caching policies for uploaded content
/// 
/// The settings are used by the S3 service to configure the AmazonS3Client for media file operations
/// including presigned URL generation for client-side uploads and secure file access.
/// </remarks>
public sealed class AwsSettings
{
    /// <summary>
    /// Gets the configuration section name for AWS S3 settings.
    /// </summary>
    public const string Section = "AwsS3";

    /// <summary>
    /// Gets or sets the AWS region where the S3 bucket is located (e.g., "us-east-1", "eu-west-1").
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the S3 bucket used for media storage (typically "expert-bridge-media").
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the AWS access key ID for authenticating S3 operations.
    /// </summary>
    public string AwsKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the AWS secret access key for authenticating S3 operations.
    /// </summary>
    public string AwsSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the public URL of the S3 bucket for constructing direct file access URLs.
    /// </summary>
    public string BucketUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the maximum allowed file size in bytes for uploads to prevent storage abuse.
    /// </summary>
    public long MaxFileSize { get; set; }

    /// <summary>
    /// Gets or sets the Cache-Control header value for uploaded files to optimize CDN caching behavior.
    /// </summary>
    public string CacheControl { get; set; } = string.Empty;
}
