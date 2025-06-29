// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.JobPostingTags
{
    public class JobPostingTagEntityConfiguration : IEntityTypeConfiguration<JobPostingTag>
    {
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
}
