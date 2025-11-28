// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.Media.ProfileMedia;

/// <summary>
///     Represents a media attachment associated with a user profile.
/// </summary>
/// <remarks>
///     Profile media includes items like profile pictures, cover photos, or portfolio pieces stored in AWS S3.
/// </remarks>
public sealed class ProfileMedia : MediaObject
{
    // Foreign keys
    /// <summary>
    ///     Gets or sets the unique identifier of the profile this media belongs to.
    /// </summary>
    public string ProfileId { get; set; }

    // Navigation properties
    /// <summary>
    ///     Gets or sets the profile this media is attached to.
    /// </summary>
    public Profile Profile { get; set; }
}
