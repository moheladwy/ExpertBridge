// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkills;

/// <summary>
///     Configures the Entity Framework Core mapping for the <see cref="ProfileSkill" /> entity.
/// </summary>
public class ProfileSkillEntityConfiguration : IEntityTypeConfiguration<ProfileSkill>
{
    /// <summary>
    ///     Configures the entity mapping, composite key, and relationships for profile skills.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<ProfileSkill> builder)
    {
        builder.HasKey(ps => new { ps.ProfileId, ps.SkillId });

        builder.HasOne(ps => ps.Profile)
            .WithMany(p => p.ProfileSkills)
            .HasForeignKey(ps => ps.ProfileId);

        builder.HasOne(ps => ps.Skill)
            .WithMany(s => s.ProfileSkills)
            .HasForeignKey(ps => ps.SkillId);
    }
}
