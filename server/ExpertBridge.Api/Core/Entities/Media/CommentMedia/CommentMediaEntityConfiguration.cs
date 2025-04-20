// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Media.CommentMedia;

public class CommentMediaEntityConfiguration : IEntityTypeConfiguration<CommentMedia>
{
    public void Configure(EntityTypeBuilder<CommentMedia> builder)
    {
        //builder.HasKey(x => x.Id);

        //builder.Property(x => x.Id)
        //    .IsRequired()
        //    .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength);

        // Comment relationship (One-to-One)
        builder.HasOne(x => x.Comment)
            .WithOne(x => x.Media)
            .HasForeignKey<CommentMedia>(x => x.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Media relationship (One-to-One)
        //builder.HasOne(x => x.Media)
        //    .WithOne(x => x.Comment)
        //    .HasForeignKey<CommentMedia>(x => x.MediaId)
        //    .OnDelete(DeleteBehavior.Cascade);

        // CommentId and MediaId must be unique.
        // This is a one-to-one relationship.
        // A comment can have only one media.
        // A media can belong to only one comment.
        //builder.HasIndex(x => x.CommentId).IsUnique();
        //builder.HasIndex(x => x.MediaId).IsUnique();
    }
}
