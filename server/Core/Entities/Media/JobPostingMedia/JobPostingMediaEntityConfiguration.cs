using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Entities.Media.JobPostingMedia;

public class JobPostingMediaEntityConfiguration : IEntityTypeConfiguration<JobPostingMedia>
{
    public void Configure(EntityTypeBuilder<JobPostingMedia> builder)
    {
        builder.ConfigureAbstractMedia();
    }
}
