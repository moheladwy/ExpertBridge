// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

/// <summary>
/// Represents the response DTO for AWS S3 presigned URL generation.
/// </summary>
/// <remarks>
/// Presigned URLs allow clients to upload files directly to S3 without exposing AWS credentials.
/// The URL is temporary and expires after a configured time period.
/// </remarks>
public class PresignedUrlResponse
{
    /// <summary>
    /// Gets or sets the presigned URL for uploading to S3.
    /// </summary>
    /// <remarks>
    /// This URL includes authentication parameters and can be used directly with an HTTP PUT request.
    /// </remarks>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the S3 object key (path) where the file will be stored.
    /// </summary>
    /// <remarks>
    /// This key should be used to reference the uploaded file after the upload completes.
    /// </remarks>
    public string Key { get; set; }
}
