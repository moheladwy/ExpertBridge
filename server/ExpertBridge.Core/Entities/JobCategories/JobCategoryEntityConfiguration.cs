

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.JobCategories;

public class JobCategoryEntityConfiguration : IEntityTypeConfiguration<JobCategory>
{
    public void Configure(EntityTypeBuilder<JobCategory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(JobCategoryEntityConstraints.MaxNameLength);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(JobCategoryEntityConstraints.MaxDescriptionLength);
    }
}
