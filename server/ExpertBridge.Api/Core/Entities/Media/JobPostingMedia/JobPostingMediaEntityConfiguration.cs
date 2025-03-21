// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Media.JobPostingMedia;

public class JobPostingMediaEntityConfiguration : IEntityTypeConfiguration<JobPostingMedia>
{
    public void Configure(EntityTypeBuilder<JobPostingMedia> builder)
    {
        builder.HasKey(jpm => jpm.Id);

        builder.Property(jpm => jpm.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.HasOne(jpm => jpm.JobPosting)
            .WithMany(jp => jp.Medias)
            .HasForeignKey(jpm => jpm.JobPostingId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(jpm => jpm.Media)
            .WithOne(m => m.JobPosting)
            .HasForeignKey<JobPostingMedia>(jpm => jpm.MediaId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // MediaId index (Unique),
        // because one media can only belong to one JobPosting.
        // But one jobPosting can have many media (One-to-Many).
        // So, that's why we need to make MediaId unique.
        // But JobPostingId can be the same for many JobPostingMedia.
        builder.HasIndex(jpm => jpm.MediaId);
    }
}
