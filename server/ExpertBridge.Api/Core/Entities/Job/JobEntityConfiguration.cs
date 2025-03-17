using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Job;

public class JobEntityConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ActualCost)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.StartedAt)
            .IsRequired();

        builder.Property(x => x.EndedAt)
            .IsRequired(false);

        // JobStatus (One-to-One)
        builder.HasOne(j => j.Status)
            .WithMany(s => s.Jobs)
            .HasForeignKey(j => j.JobStatusId)
            .IsRequired();

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
    }
}
