// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags;

/// <summary>
///     Configures the Entity Framework Core mapping for the <see cref="PostTag" /> entity.
/// </summary>
public class PostTagEntityConfiguration : IEntityTypeConfiguration<PostTag>
{
    /// <summary>
    ///     Configures the entity mapping, composite key, and relationships for post tags.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
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
