// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.ModerationReports;

/// <summary>
/// Configures the Entity Framework Core mapping for the <see cref="ModerationReport"/> entity.
/// </summary>
public class ModerationReportEntityConfiguration : IEntityTypeConfiguration<ModerationReport>
{
    /// <summary>
    /// Configures the entity mapping, relationships, and database constraints for moderation reports.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<ModerationReport> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ContentType)
            .HasConversion<string>();

        builder.HasIndex(x => x.IsNegative);
        builder.HasIndex(x => x.ContentId);
        builder.HasIndex(x => new { x.IsNegative, x.ContentId });
    }
}
