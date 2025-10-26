// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ChatParticipants;

/// <summary>
/// Configures the Entity Framework Core mapping for the <see cref="ChatParticipant"/> entity.
/// </summary>
public class ChatParticipantEntityConfiguration : IEntityTypeConfiguration<ChatParticipant>
{
    /// <summary>
    /// Configures the entity mapping, composite key, and relationships for chat participants.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<ChatParticipant> builder)
    {
        builder.HasKey(x => new { x.ChatId, x.ProfileId });

        //builder.HasOne(cp => cp.Chat)
        //    .WithMany(c => c.Participants)
        //    .HasForeignKey(cp => cp.ChatId)
        //    .IsRequired();

        builder.HasOne(cp => cp.Profile)
            .WithOne(p => p.ChatParticipant)
            .HasForeignKey<ChatParticipant>(cp => cp.ProfileId)
            .IsRequired();
    }
}
