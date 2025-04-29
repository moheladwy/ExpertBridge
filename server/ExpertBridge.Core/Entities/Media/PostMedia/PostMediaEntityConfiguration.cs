

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Media.PostMedia;

public class PostMediaEntityConfiguration : IEntityTypeConfiguration<PostMedia>
{
    public void Configure(EntityTypeBuilder<PostMedia> builder)
    {
        builder.ConfigureAbstractMedia();
    }
}
