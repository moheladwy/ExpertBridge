// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkills;

public class ProfileSkillEntityConfiguration : IEntityTypeConfiguration<ProfileSkill>
{
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
