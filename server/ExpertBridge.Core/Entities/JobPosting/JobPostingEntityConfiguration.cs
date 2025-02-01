using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.JobPosting;

public class JobPostingEntityConfiguration : IEntityTypeConfiguration<JobPosting>
{
    public void Configure(EntityTypeBuilder<JobPosting> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(JobPostingEntityConstraints.MaxTitleLength);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(JobPostingEntityConstraints.MaxDescriptionLength);

        builder.Property(x => x.Cost)
            .IsRequired()
            .HasPrecision(18, 2);

        // TODO: Add AuthorId, AreaId, CategoryId foreign keys.
    }
}
