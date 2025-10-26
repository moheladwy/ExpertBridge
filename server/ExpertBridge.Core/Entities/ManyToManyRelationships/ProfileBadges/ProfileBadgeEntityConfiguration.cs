// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileBadges;

/// <summary>
/// Configures the Entity Framework Core mapping for the <see cref="ProfileBadge"/> entity.
/// </summary>
public class ProfileBadgeEntityConfiguration : IEntityTypeConfiguration<ProfileBadge>
{
    /// <summary>
    /// Configures the entity mapping, composite key, and relationships for profile badges.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
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
