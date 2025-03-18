// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ProfileBadges;

public class ProfileBadgeEntityConfiguration : IEntityTypeConfiguration<ProfileBadge>
{
    public void Configure(EntityTypeBuilder<ProfileBadge> builder)
    {
        builder.HasKey(pb => new { pb.ProfileId, pb.BadgeId });

        builder.HasOne(pb => pb.Profile)
            .WithMany(p => p.ProfileBadges)
            .HasForeignKey(pb => pb.ProfileId);

        builder.HasOne(pb => pb.Badge)
            .WithMany(b => b.ProfileBadges)
            .HasForeignKey(pb => pb.BadgeId);
    }
}
