using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Entities.Media.ProfileMedia;

public class ProfileMediaEntityConfiguration : IEntityTypeConfiguration<ProfileMedia>
{
    public void Configure(EntityTypeBuilder<ProfileMedia> builder)
    {
        builder.ConfigureAbstractMedia();
    }
}
