using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.JobCategory;

public class JobCategoryEntityConfiguration : IEntityTypeConfiguration<JobCategory>
{
    public void Configure(EntityTypeBuilder<JobCategory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(JobCategoryEntityConstraints.MaxNameLength);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(JobCategoryEntityConstraints.MaxDescriptionLength);
    }
}
