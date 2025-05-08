using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Notifications
{
    public class NotificationEntityConfiguration : IEntityTypeConfiguration<Notification>
    {
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
}
