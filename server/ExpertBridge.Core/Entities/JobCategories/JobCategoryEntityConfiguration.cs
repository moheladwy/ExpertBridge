// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.JobCategories;

/// <summary>
/// Configures the Entity Framework Core mapping for the <see cref="JobCategory"/> entity.
/// </summary>
public class JobCategoryEntityConfiguration : IEntityTypeConfiguration<JobCategory>
{
    /// <summary>
    /// Configures the entity mapping, relationships, and database constraints for job categories.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<JobCategory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(JobCategoryEntityConstraints.MaxNameLength);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(JobCategoryEntityConstraints.MaxDescriptionLength);
    }
}
