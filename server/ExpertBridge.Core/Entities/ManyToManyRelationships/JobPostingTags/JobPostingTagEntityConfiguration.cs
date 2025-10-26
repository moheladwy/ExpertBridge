// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.JobPostingTags;

/// <summary>
///     Configures the Entity Framework Core mapping for the <see cref="JobPostingTag" /> entity.
/// </summary>
public class JobPostingTagEntityConfiguration : IEntityTypeConfiguration<JobPostingTag>
{
    /// <summary>
    ///     Configures the entity mapping, composite key, and relationships for job posting tags.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<JobPostingTag> builder)
    {
        // Configure composite key
        builder.HasKey(pt => new { pt.JobPostingId, pt.TagId });

        // Configure relationship with Post
        builder.HasOne(pt => pt.JobPosting)
            .WithMany(p => p.JobPostingTags)
            .HasForeignKey(pt => pt.JobPostingId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with Tag
        builder.HasOne(pt => pt.Tag)
            .WithMany(t => t.JobPostingTags)
            .HasForeignKey(pt => pt.TagId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
