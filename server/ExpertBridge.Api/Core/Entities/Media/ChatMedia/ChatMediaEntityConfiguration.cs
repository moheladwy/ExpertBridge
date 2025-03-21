// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Media.ChatMedia;

public class ChatMediaEntityConfiguration : IEntityTypeConfiguration<ChatMedia>
{
    public void Configure(EntityTypeBuilder<ChatMedia> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        // Chat relationship (One-to-Many)
        builder.HasOne(x => x.Chat)
            .WithMany(x => x.Medias)
            .HasForeignKey(x => x.ChatId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Media relationship (One-to-One)
        builder.HasOne(x => x.Media)
            .WithOne(x => x.Chat)
            .HasForeignKey<ChatMedia>(x => x.MediaId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // MediaId index (Unique),
        // because one media can only belong to one chat.
        // But one chat can have many media (One-to-Many).
        // So, that's why we need to make MediaId unique.
        // But ChatId can be the same for many ChatMedia.
        builder.HasIndex(x => x.MediaId).IsUnique();
    }
}
