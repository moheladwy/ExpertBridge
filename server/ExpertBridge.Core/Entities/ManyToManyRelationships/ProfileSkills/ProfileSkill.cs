// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkills;

public class ProfileSkill
{
    public string ProfileId { get; set; }
    public string SkillId { get; set; }

    // Navigation properties
    public Profiles.Profile Profile { get; set; }
    public Skills.Skill Skill { get; set; }
}
