using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Entities.JobReviews;

public class JobReviewEntityConfiguration : IEntityTypeConfiguration<JobReview>
{
    public void Configure(EntityTypeBuilder<JobReview> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(JobReviewEntityConstraints.MaxReviewLength);

        builder.Property(x => x.Rating)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.LastModified)
            .IsRequired(false)
            .ValueGeneratedOnAddOrUpdate();

        builder.HasOne(jr => jr.Job)
            .WithOne(j => j.Review)
            .HasForeignKey<JobReview>(j => j.JobId)
            .IsRequired();
    }
}
