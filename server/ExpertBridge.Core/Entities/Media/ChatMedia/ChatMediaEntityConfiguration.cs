

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Media.ChatMedia;

public class ChatMediaEntityConfiguration : IEntityTypeConfiguration<ChatMedia>
{
    public void Configure(EntityTypeBuilder<ChatMedia> builder)
    {
        builder.ConfigureAbstractMedia();
    }
}
