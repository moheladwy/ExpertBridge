// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.JobOffers;

/// <summary>
/// Configures the entity mapping for the <see cref="JobOffer"/> entity.
/// Defines the database schema for job offers including primary key, description constraints, relationships with worker and author profiles, and associated job application references.
/// </summary>
public class JobOfferEntityConfiguration : IEntityTypeConfiguration<JobOffer>
{
    /// <summary>
    /// Configures the entity type for JobOffer, including primary key, worker index for query optimization, description max length constraint, and relationships with Profile entities (as worker and author).
    /// </summary>
    /// <param name="builder">The builder to be used to configure the JobOffer entity.</param>
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
