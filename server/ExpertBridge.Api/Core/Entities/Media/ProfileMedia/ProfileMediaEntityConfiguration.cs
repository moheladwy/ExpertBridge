// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Media.ProfileMedia;

public class ProfileMediaEntityConfiguration : IEntityTypeConfiguration<ProfileMedia>
{
    public void Configure(EntityTypeBuilder<ProfileMedia> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        // Media relationship (One-to-one)
        builder.HasOne(x => x.Media)
            .WithOne(m => m.Profile)
            .HasForeignKey<ProfileMedia>(x => x.MediaId)
            .IsRequired();

        // Profile relationship (One-to-Many)
        builder.HasOne(m => m.Profile)
            .WithMany(p => p.Medias)
            .HasForeignKey(m => m.ProfileId)
            .IsRequired();

        // MediaId index (Unique),
        // because one media can only belong to one profile.
        // But one profile can have many media (One-to-Many).
        // So, that's why we need to make MediaId unique.
        // But ProfileId can be the same for many ProfileMedia.
        builder.HasIndex(x => x.MediaId).IsUnique();

    }
}
