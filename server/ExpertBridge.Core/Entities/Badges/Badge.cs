// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileBadges;

namespace ExpertBridge.Core.Entities.Badges;

public class Badge
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<ProfileBadge> ProfileBadges { get; set; } = [];
}
