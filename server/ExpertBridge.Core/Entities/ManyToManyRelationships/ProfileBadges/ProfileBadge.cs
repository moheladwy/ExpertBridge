// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Badges;
using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileBadges;

/// <summary>
///     Represents a many-to-many relationship between profiles and badges.
/// </summary>
/// <remarks>
///     Profile badges recognize achievements and qualifications earned by users on the platform.
///     Badges contribute to professional credibility and are displayed on user profiles to signal expertise.
/// </remarks>
public class ProfileBadge
{
    /// <summary>
    ///     Gets or sets the unique identifier of the profile.
    /// </summary>
    public string ProfileId { get; set; }

    /// <summary>
    ///     Gets or sets the profile that earned the badge.
    /// </summary>
    public Profile Profile { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the badge.
    /// </summary>
    public string BadgeId { get; set; }

    /// <summary>
    ///     Gets or sets the badge earned by the profile.
    /// </summary>
    public Badge Badge { get; set; }
}
