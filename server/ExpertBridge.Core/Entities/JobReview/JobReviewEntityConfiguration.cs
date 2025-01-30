using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.JobReview;

public class JobReviewEntityConfiguration : IEntityTypeConfiguration<JobReview>
{
    public void Configure(EntityTypeBuilder<JobReview> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(JobReviewEntityConstraints.MaxReviewLength);

        builder.Property(x => x.Rating)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();
    }
}
