// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Data.EntityConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Media.ProfileMedia;

public class ProfileMediaEntityConfiguration : IEntityTypeConfiguration<ProfileMedia>
{
    public void Configure(EntityTypeBuilder<ProfileMedia> builder)
    {
        builder.ConfigureAbstractMedia();

        // Profile relationship (One-to-Many)
        builder.HasOne(m => m.Profile)
            .WithMany(p => p.Medias)
            .HasForeignKey(m => m.ProfileId)
            .IsRequired();
    }
}
