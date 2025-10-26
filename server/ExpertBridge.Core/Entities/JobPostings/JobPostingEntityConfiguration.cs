// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.EntityConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.JobPostings;

/// <summary>
///     Configures the Entity Framework Core mapping for the <see cref="JobPosting" /> entity.
/// </summary>
public class JobPostingEntityConfiguration : IEntityTypeConfiguration<JobPosting>
{
    /// <summary>
    ///     Configures the entity mapping, relationships, indexes, and database constraints for job postings.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<JobPosting> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(JobPostingEntityConstraints.MaxTitleLength);

        builder.HasIndex(x => x.Title);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(JobPostingEntityConstraints.MaxContentLength);

        builder.Property(x => x.Budget)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        // Profile relationship (One-to-Many)
        builder.HasOne(j => j.Author)
            .WithMany(p => p.JobPostings)
            .HasForeignKey(j => j.AuthorId)
            .IsRequired();

        // Area relationship (One-to-Many)
        //builder.HasOne(j => j.Area)
        //    .WithMany(a => a.JobPostings)
        //    .HasForeignKey(j => j.AreaId)
        //    .IsRequired(false);

        // JobCategory relationship (One-to-Many)
        //builder.HasOne(j => j.Category)
        //    .WithMany(c => c.JobPostings)
        //    .HasForeignKey(j => j.CategoryId)
        //    .IsRequired();

        // Job relationship (One-to-One)
        //builder.HasOne(jp => jp.Job)
        //    .WithOne(j => j.JobPosting)
        //    .HasForeignKey<Jobs.Job>(j => j.JobPostingId)
        //    .IsRequired(false);

        // Configure one-to-many relationship with JobPostingMedia
        builder.HasMany(jp => jp.Medias)
            .WithOne(m => m.JobPosting)
            .HasForeignKey(m => m.JobPostingId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false)
            ;

        builder.Property(p => p.Embedding)
            .HasColumnType(ColumnTypes.Vector1024)
            .IsRequired(false);

        builder
            .HasIndex(p => p.Embedding)
            .HasMethod(IndexMethods.Hnsw)
            .HasOperators("vector_cosine_ops")
            .HasStorageParameter("m", 64) // Example, tune this
            .HasStorageParameter("ef_construction", 128) // Example, tune this
            ;
    }
}
