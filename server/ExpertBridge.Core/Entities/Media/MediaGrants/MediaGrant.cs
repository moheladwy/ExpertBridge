// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.Media.MediaGrants;

/// <summary>
/// Represents a temporary access grant for media files stored in AWS S3.
/// </summary>
/// <remarks>
/// Media grants control access to S3 objects through presigned URLs with lifecycle management.
/// Grants can be placed on hold, activated, and soft-deleted to manage secure file access.
/// </remarks>
public class MediaGrant : ISoftDeletable
{
    /// <summary>
    /// Gets or sets the unique identifier of the media grant.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the S3 object key this grant provides access to.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the grant is temporarily on hold.
    /// </summary>
    public bool OnHold { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the grant is currently active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the grant was initially created.
    /// </summary>
    public DateTime GrantedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the grant was activated.
    /// </summary>
    public DateTime? ActivatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this grant has been soft-deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this grant was soft-deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
