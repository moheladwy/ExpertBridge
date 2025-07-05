// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Messages
{
    public class MessageEntityConfiguration : IEntityTypeConfiguration<Message>
    {
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
}
