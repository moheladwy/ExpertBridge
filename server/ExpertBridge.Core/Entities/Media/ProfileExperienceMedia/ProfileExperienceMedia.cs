// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.ProfileExperiences;

namespace ExpertBridge.Core.Entities.Media.ProfileExperienceMedia;

/// <summary>
///     Represents a media attachment associated with a profile experience entry.
/// </summary>
/// <remarks>
///     Profile experience media can include certificates, project screenshots, or other evidence stored in AWS S3
///     to support work history claims.
/// </remarks>
public sealed class ProfileExperienceMedia : MediaObject
{
    // Foreign keys
    /// <summary>
    ///     Gets or sets the unique identifier of the profile experience this media belongs to.
    /// </summary>
    public string ProfileExperienceId { get; set; }

    // Navigation properties
    /// <summary>
    ///     Gets or sets the profile experience this media is attached to.
    /// </summary>
    public ProfileExperience ProfileExperience { get; set; }
}
