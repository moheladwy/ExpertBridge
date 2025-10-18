// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Skills;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkills;

/// <summary>
/// Represents a many-to-many relationship between profiles and skills.
/// </summary>
/// <remarks>
/// Profile skills define the professional capabilities users declare on their profiles.
/// These skills are used for job matching, search filtering, and reputation calculations based on completed work.
/// </remarks>
public class ProfileSkill
{
    /// <summary>
    /// Gets or sets the unique identifier of the profile.
    /// </summary>
    public string ProfileId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the skill.
    /// </summary>
    public string SkillId { get; set; } = null!;

    // Navigation properties
    /// <summary>
    /// Gets or sets the profile that possesses the skill.
    /// </summary>
    public Profile Profile { get; set; } = null!;

    /// <summary>
    /// Gets or sets the skill associated with the profile.
    /// </summary>
    public Skill Skill { get; set; } = null!;
}
