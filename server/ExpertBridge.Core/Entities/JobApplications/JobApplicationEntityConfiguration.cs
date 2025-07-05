// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertBridge.Core.Entities.JobPostings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.JobApplications
{
    public class JobApplicationEntityConfiguration : IEntityTypeConfiguration<JobApplication>
    {
        public void Configure(EntityTypeBuilder<JobApplication> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
                .ValueGeneratedOnAdd();

            builder.HasIndex(x => x.JobPostingId);

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.CoverLetter)
                .HasMaxLength(GlobalEntitiesConstraints.MaxCoverLetterLength);

            // Profile relationship (One-to-Many)
            builder.HasOne(j => j.Applicant)
                .WithMany(p => p.JobApplications)
                .HasForeignKey(j => j.ApplicantId)
                .IsRequired();

            builder.HasOne(j => j.JobPosting)
                .WithMany(p => p.JobApplications)
                .HasForeignKey(j => j.JobPostingId)
                .IsRequired();
        }
    }
}
