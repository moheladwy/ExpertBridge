// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.EntityConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Skills;

public class SkillEntityConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(SkillEntityConstraints.MaxNameLength);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasMaxLength(SkillEntityConstraints.MaxDescriptionLength);

        builder.Property(p => p.Embedding)
            .HasColumnType(ColumnTypes.Vector1024)
            .IsRequired(false);

        builder
            .HasIndex(s => s.Embedding)
            .HasMethod(IndexMethods.Hnsw)
            .HasOperators("vector_cosine_ops")
            .HasStorageParameter("m", 64) // Example, tune this
            .HasStorageParameter("ef_construction", 128) // Example, tune this
            ;
    }
}
