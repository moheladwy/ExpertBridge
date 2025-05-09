using ExpertBridge.Core.Entities.JobReviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Jobs;

public class JobEntityConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(GlobalEntitiesConstraints.MaxTitleLength);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(GlobalEntitiesConstraints.MaxDescriptionLength);

        builder.Property(x => x.ActualCost)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.StartedAt)
            .IsRequired(false);

        builder.Property(x => x.EndedAt)
            .IsRequired(false);

        

        builder.Property(j => j.Status)
            .IsRequired()
            .HasConversion<string>() 
            .HasMaxLength(GlobalEntitiesConstraints.MaxEnumsLength);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        // Author Profile (One-to-Many)
        builder.HasOne(j => j.Author)
            .WithMany(p => p.JobsAsAuthor)
            .HasForeignKey(j => j.AuthorId)
            .IsRequired();

        

        // Worker Profile (One-to-Many)
        builder.HasOne(j => j.Worker)
            .WithMany(p => p.JobsAsWorker)
            .HasForeignKey(j => j.WorkerId)
            .IsRequired();

        // JobPosting relationship (One-to-One)
        builder.HasOne(j => j.JobPosting)
            .WithOne(jp => jp.Job)
            .HasForeignKey<Job>(j => j.JobPostingId)
            .IsRequired(false);

        // JobReview (One-to-One)
        builder.HasOne(j => j.Review)
            .WithOne(r => r.Job)
            .HasForeignKey<JobReview>(r => r.JobId)
            .IsRequired(false);
    }
}
