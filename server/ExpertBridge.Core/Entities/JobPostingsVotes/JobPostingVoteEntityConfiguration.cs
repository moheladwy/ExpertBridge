// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertBridge.Core.Entities.PostVotes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.JobPostingsVotes
{
    public class JobPostingVoteEntityConfiguration : IEntityTypeConfiguration<JobPostingVote>
    {
        public void Configure(EntityTypeBuilder<JobPostingVote> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.IsUpvote)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .ValueGeneratedOnAdd();

            // Configure relationship with Profile
            builder.HasOne(v => v.Profile)
                .WithMany(p => p.JobPostingVotes)
                .HasForeignKey(v => v.ProfileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with JobPosting
            builder.HasOne(v => v.JobPosting)
                .WithMany(p => p.Votes)
                .HasForeignKey(v => v.JobPostingId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the unique constraint
            builder.HasIndex(x => new { x.ProfileId, x.JobPostingId })
                .IsUnique();
        }
    }
}
