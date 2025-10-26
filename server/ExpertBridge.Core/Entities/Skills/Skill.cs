// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkills;

namespace ExpertBridge.Core.Entities.Skills;

/// <summary>
///     Represents a professional skill that can be associated with user profiles.
/// </summary>
/// <remarks>
///     Skills are used to match professionals with relevant job opportunities and to showcase expertise on profiles.
/// </remarks>
public class Skill
{
    /// <summary>
    ///     Gets or sets the unique identifier for the skill.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    ///     Gets or sets the name of the skill.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets a description of what the skill encompasses.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the collection of profile-skill relationships.
    /// </summary>
    /// <remarks>
    ///     This relationship tracks which profiles have claimed this skill.
    /// </remarks>
    public ICollection<ProfileSkill> ProfileSkills { get; set; } = [];
}
