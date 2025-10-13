// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkills;

namespace ExpertBridge.Core.Entities.Skills;

public class Skill
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public string? Description { get; set; }

    public ICollection<ProfileSkill> ProfileSkills { get; set; } = [];
}
