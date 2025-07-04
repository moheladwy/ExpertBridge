// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.JobOffers
{
    public class JobOfferEntityConfiguration : IEntityTypeConfiguration<JobOffer>
    {
        public void Configure(EntityTypeBuilder<JobOffer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
                .ValueGeneratedOnAdd();

            builder.HasIndex(x => x.WorkerId);

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Description)
                .HasMaxLength(GlobalEntitiesConstraints.MaxDescriptionLength);

            // Profile relationship (One-to-Many)
            builder.HasOne(j => j.Worker)
                .WithMany(w => w.ReceivedJobOffers)
                .HasForeignKey(j => j.WorkerId)
                .IsRequired();

            builder.HasOne(j => j.Author)
                .WithMany(p => p.AuthoredJobOffers)
                .HasForeignKey(j => j.AuthorId)
                .IsRequired();
        }
    }
}
