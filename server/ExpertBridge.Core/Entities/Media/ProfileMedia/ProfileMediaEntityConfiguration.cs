

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Media.ProfileMedia;

public class ProfileMediaEntityConfiguration : IEntityTypeConfiguration<ProfileMedia>
{
    public void Configure(EntityTypeBuilder<ProfileMedia> builder)
    {
        builder.ConfigureAbstractMedia();
    }
}
