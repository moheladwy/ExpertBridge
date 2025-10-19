// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities;

/// <summary>
/// Represents metadata information for uploaded files stored in AWS S3.
/// </summary>
/// <remarks>
/// Used to track file details for media attachments including content type, S3 key, size, and timestamps.
/// This class is typically used in conjunction with presigned URL generation for client-side uploads.
/// </remarks>
public class FileMetadata
{
    /// <summary>
    /// Gets or sets the MIME content type of the file (e.g., "image/jpeg", "video/mp4").
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// Gets or sets the S3 object key where the file is stored.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the type category of the file (e.g., "image", "video", "document").
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the original file name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Gets or sets the file extension (e.g., ".jpg", ".mp4").
    /// </summary>
    public string Extension { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the file was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the file was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
