// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Messages;

/// <summary>
/// Configures the Entity Framework Core mapping for the <see cref="Message"/> entity.
/// </summary>
public class MessageEntityConfiguration : IEntityTypeConfiguration<Message>
{
    /// <summary>
    /// Configures the entity mapping, relationships, and database constraints for messages.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Content)
            .HasMaxLength(GlobalEntitiesConstraints.MaxContentLetterLength);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Sender)
            .WithMany(p => p.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .IsRequired();

        builder.HasOne(x => x.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId)
            .IsRequired();
    }
}
