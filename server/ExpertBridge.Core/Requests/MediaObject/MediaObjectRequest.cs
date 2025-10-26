// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.MediaObject;

/// <summary>
/// Represents a request containing media object information for attachment to posts or comments.
/// </summary>
/// <remarks>
/// This DTO is used after files have been uploaded to S3 via presigned URLs.
/// The key and type are used to create media attachment records.
/// </remarks>
public class MediaObjectRequest
{
    /// <summary>
    /// Gets or sets the S3 object key (path) where the file is stored.
    /// </summary>
    /// <remarks>
    /// This key is obtained from the presigned URL generation response.
    /// </remarks>
    public required string Key { get; set; }

    /// <summary>
    /// Gets or sets the MIME type of the media file.
    /// </summary>
    /// <remarks>
    /// Examples include "image/jpeg", "video/mp4", "application/pdf", etc.
    /// </remarks>
    public required string Type { get; set; }
}
