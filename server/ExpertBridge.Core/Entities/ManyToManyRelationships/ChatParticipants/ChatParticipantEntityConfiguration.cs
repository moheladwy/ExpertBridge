// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ChatParticipants;

public class ChatParticipantEntityConfiguration : IEntityTypeConfiguration<ChatParticipant>
{
    public void Configure(EntityTypeBuilder<ChatParticipant> builder)
    {
        builder.HasKey(x => new { x.ChatId, x.ProfileId });

        builder.HasOne(cp => cp.Chat)
            .WithMany(c => c.Participants)
            .HasForeignKey(cp => cp.ChatId)
            .IsRequired();

        builder.HasOne(cp => cp.Profile)
            .WithOne(p => p.ChatParticipant)
            .HasForeignKey<ChatParticipant>(cp => cp.ProfileId)
            .IsRequired();
    }

}
