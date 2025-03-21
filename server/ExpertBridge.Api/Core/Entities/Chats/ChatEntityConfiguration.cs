// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Chats;

public class ChatEntityConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.EndedAt)
            .IsRequired(false);

        // Configure participants relationship
        builder.HasMany(c => c.Participants)
            .WithOne(cp => cp.Chat)
            .HasForeignKey(cp => cp.ChatId)
            .IsRequired();

        // Enforce exactly 2 participants
        builder.Navigation(c => c.Participants)
            .AutoInclude();
    }
}
