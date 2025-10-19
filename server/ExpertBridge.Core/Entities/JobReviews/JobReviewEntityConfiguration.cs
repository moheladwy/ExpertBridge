// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.JobReviews;

/// <summary>
/// Configures the Entity Framework Core mapping for the <see cref="JobReview"/> entity.
/// </summary>
public class JobReviewEntityConfiguration : IEntityTypeConfiguration<JobReview>
{
    /// <summary>
    /// Configures the entity mapping, relationships, and database constraints for job reviews.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<JobReview> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(JobReviewEntityConstraints.MaxReviewLength);

        builder.Property(x => x.Rating)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.LastModified)
            .IsRequired(false)
            .ValueGeneratedOnAddOrUpdate();

        builder.HasOne(jr => jr.Job)
            .WithOne(j => j.Review)
            .HasForeignKey<JobReview>(j => j.JobId)
            .IsRequired();
    }
}
