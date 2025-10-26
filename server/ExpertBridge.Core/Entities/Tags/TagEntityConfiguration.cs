// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Tags;

/// <summary>
///     Configures the Entity Framework Core mapping for the <see cref="Tag" /> entity.
/// </summary>
public class TagEntityConfiguration : IEntityTypeConfiguration<Tag>
{
    /// <summary>
    ///     Configures the entity mapping, relationships, and database constraints for tags.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.EnglishName)
            .IsRequired()
            .HasMaxLength(TagEntityConstraints.MaxNameLength);

        builder.HasIndex(x => x.EnglishName).IsUnique();
        builder.HasIndex(x => x.ArabicName).IsUnique();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(TagEntityConstraints.MaxDescriptionLength);
    }
}
