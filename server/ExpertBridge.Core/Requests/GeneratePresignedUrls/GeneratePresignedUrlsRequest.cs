// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;

namespace ExpertBridge.Core.Requests.GeneratePresignedUrls;

/// <summary>
/// Represents a request to generate AWS S3 presigned URLs for file uploads.
/// </summary>
/// <remarks>
/// This request is used to obtain temporary upload URLs that allow clients to upload files
/// directly to S3 without exposing AWS credentials.
/// </remarks>
public class GeneratePresignedUrlsRequest
{
    /// <summary>
    /// Gets or sets the collection of file metadata for which presigned URLs should be generated.
    /// </summary>
    public List<FileMetadata> Files { get; set; }
}
