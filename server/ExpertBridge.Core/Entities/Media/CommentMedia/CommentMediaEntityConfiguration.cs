

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Media.CommentMedia;

public class CommentMediaEntityConfiguration : IEntityTypeConfiguration<CommentMedia>
{
    public void Configure(EntityTypeBuilder<CommentMedia> builder)
    {
        builder.ConfigureAbstractMedia();
    }
}
