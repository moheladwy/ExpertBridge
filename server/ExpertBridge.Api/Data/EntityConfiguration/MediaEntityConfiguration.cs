// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities;
using ExpertBridge.Api.Core.Entities.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Data.EntityConfiguration
{
    public static class MediaEntityConfiguration
    {
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
                .HasFilter(IndexFilters.NotDeleted)
                .IsUnique();

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.LastModified)
                .IsRequired(false)
                .ValueGeneratedOnAddOrUpdate();
        }
    }
}
