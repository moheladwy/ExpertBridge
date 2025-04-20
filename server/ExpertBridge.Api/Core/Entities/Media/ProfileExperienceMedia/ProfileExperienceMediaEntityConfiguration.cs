// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Media.ProfileExperienceMedia;

public class ProfileExperienceMediaEntityConfiguration : IEntityTypeConfiguration<ProfileExperienceMedia>
{
    public void Configure(EntityTypeBuilder<ProfileExperienceMedia> builder)
    {
        //builder.HasKey(pem => pem.Id);

        //builder.Property(pem => pem.Id)
        //    .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
        //    .ValueGeneratedOnAdd();

        // ProfileExperience relationship (One-to-Many)
        builder.HasOne(pem => pem.ProfileExperience)
            .WithMany(profileExperience => profileExperience.Medias)
            .HasForeignKey(pem => pem.ProfileExperienceId)
            .IsRequired();

        // Media relationship (One-to-one)
        //builder.HasOne(pem => pem.Media)
        //    .WithOne(media => media.ProfileExperience)
        //    .HasForeignKey<ProfileExperienceMedia>(pem => pem.MediaId)
        //    .IsRequired();

        // MediaId index (Unique),
        // because one media can only belong to one profile experience.
        // But one profile experience can have many media (One-to-Many).
        // So, that's why we need to make MediaId unique.
        // But ProfileExperienceId can be the same for many ProfileExperienceMedia.
        //builder.HasIndex(pem => pem.MediaId).IsUnique();
    }
}
