// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Data.EntityConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Media.JobPostingMedia;

public class JobPostingMediaEntityConfiguration : IEntityTypeConfiguration<JobPostingMedia>
{
    public void Configure(EntityTypeBuilder<JobPostingMedia> builder)
    {
        builder.ConfigureAbstractMedia();

        builder.HasOne(jpm => jpm.JobPosting)
            .WithMany(jp => jp.Medias)
            .HasForeignKey(jpm => jpm.JobPostingId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
