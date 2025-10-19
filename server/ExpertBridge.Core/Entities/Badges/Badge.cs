// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileBadges;

namespace ExpertBridge.Core.Entities.Badges;

/// <summary>
/// Represents an achievement badge that can be awarded to user profiles.
/// </summary>
/// <remarks>
/// Badges recognize user accomplishments, expertise levels, and contributions to the platform.
/// </remarks>
public class Badge
{
    /// <summary>
    /// Gets or sets the unique identifier for the badge.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the name of the badge.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the badge and criteria for earning it.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the collection of profile-badge associations.
    /// </summary>
    public ICollection<ProfileBadge> ProfileBadges { get; set; } = [];
}
