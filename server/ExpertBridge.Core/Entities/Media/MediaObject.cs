// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.Media;

/// <summary>
/// Represents the base class for media attachments stored in AWS S3.
/// </summary>
/// <remarks>
/// MediaObject is an abstract base class for various media types (post media, comment media, profile media, etc.).
/// All media files are stored in S3 and referenced by their keys.
/// </remarks>
public abstract class MediaObject : BaseModel, ISoftDeletable
{
    /// <summary>
    /// Gets or sets the original file name of the media.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the S3 object key where the media file is stored.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Gets or sets the MIME type of the media file (e.g., "image/jpeg", "video/mp4").
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the media was deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the media is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
