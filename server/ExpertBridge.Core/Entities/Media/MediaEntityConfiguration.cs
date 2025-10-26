// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Media;

/// <summary>
/// Provides shared Entity Framework Core configuration for abstract <see cref="MediaObject"/> derived entities.
/// </summary>
public static class MediaEntityConfiguration
{
    /// <summary>
    /// Configures common entity mapping and database constraints for media object entities.
    /// </summary>
    /// <typeparam name="TEntity">The media entity type that inherits from <see cref="MediaObject"/>.</typeparam>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public static void ConfigureAbstractMedia<TEntity>(
        this EntityTypeBuilder<TEntity> builder) where TEntity : MediaObject
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(MediaEntityConstraints.MaxNameLength);

        builder.Property(x => x.Key)
            .IsRequired();

        builder.HasIndex(x => x.Key)
            .IsUnique();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.LastModified)
            .IsRequired(false)
            .ValueGeneratedOnAddOrUpdate();
    }
}
