// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Data.EntityConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Media.PostMedia;

public class PostMediaEntityConfiguration : IEntityTypeConfiguration<PostMedia>
{
    public void Configure(EntityTypeBuilder<PostMedia> builder)
    {
        builder.ConfigureAbstractMedia();

        // Post relationship (One-to-Many)
        builder.HasOne(pm => pm.Post)
            .WithMany(post => post.Medias)
            .HasForeignKey(pm => pm.PostId)
            .IsRequired();
    }
}
