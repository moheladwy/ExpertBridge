// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Notifications;

/// <summary>
/// Configures the Entity Framework Core mapping for the <see cref="Notification"/> entity.
/// </summary>
public class NotificationEntityConfiguration : IEntityTypeConfiguration<Notification>
{
    /// <summary>
    /// Configures the entity mapping, relationships, and database constraints for notifications.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.RecipientId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(x => x.IsRead);
        builder.HasIndex(x => x.RecipientId);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => new { x.IsRead, x.CreatedAt });

        builder.Property(n => n.ActionUrl)
            .HasMaxLength(2000);

        builder.Property(n => n.IconUrl)
            .HasMaxLength(2000);

        builder.Property(n => n.IconActionUrl)
            .HasMaxLength(2000);
    }
}
