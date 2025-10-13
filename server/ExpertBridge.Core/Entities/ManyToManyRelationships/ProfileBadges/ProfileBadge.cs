// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Badges;
using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileBadges;

public class ProfileBadge
{
    public string ProfileId { get; set; }
    public Profile Profile { get; set; }

    public string BadgeId { get; set; }
    public Badge Badge { get; set; }
}
