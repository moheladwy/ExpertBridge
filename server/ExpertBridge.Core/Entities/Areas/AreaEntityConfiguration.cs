// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Areas;

/// <summary>
/// Configures the Entity Framework Core mapping for the <see cref="Area"/> entity.
/// </summary>
public class AreaEntityConfiguration : IEntityTypeConfiguration<Area>
{
    /// <summary>
    /// Configures the entity mapping, relationships, and database constraints for areas.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Governorate)
            .IsRequired()
            .HasMaxLength(AreaEntityConstraints.MaxGovernorateLength)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<Governorates>(v)
            );

        builder.Property(x => x.Region)
            .IsRequired()
            .HasMaxLength(AreaEntityConstraints.MaxRegionLength);

        builder.HasOne(a => a.Profile)
            .WithMany(p => p.Areas)
            .HasForeignKey(a => a.ProfileId)
            .IsRequired();
    }
}
