using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Chat.ChatParticipant;

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
