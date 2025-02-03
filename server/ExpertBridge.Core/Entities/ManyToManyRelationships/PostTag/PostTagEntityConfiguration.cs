// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.PostTag;

public class PostTagEntityConfiguration : IEntityTypeConfiguration<PostTag>
{
    public void Configure(EntityTypeBuilder<PostTag> builder)
    {
        // Configure composite key
        builder.HasKey(pt => new { pt.PostId, pt.TagId });

        // Configure relationship with Post
        builder.HasOne(pt => pt.Post)
            .WithMany(p => p.PostTags)
            .HasForeignKey(pt => pt.PostId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with Tag
        builder.HasOne(pt => pt.Tag)
            .WithMany(t => t.PostTags)
            .HasForeignKey(pt => pt.TagId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
