

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Media.ProfileExperienceMedia;

public class ProfileExperienceMediaEntityConfiguration : IEntityTypeConfiguration<ProfileExperienceMedia>
{
    public void Configure(EntityTypeBuilder<ProfileExperienceMedia> builder)
    {
        builder.ConfigureAbstractMedia();
    }
}
