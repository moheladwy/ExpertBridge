using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.JobPostings;

public class JobPostingEntityConfiguration : IEntityTypeConfiguration<JobPosting>
{
    public void Configure(EntityTypeBuilder<JobPosting> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(JobPostingEntityConstraints.MaxTitleLength);

        builder.HasIndex(x => x.Title);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(JobPostingEntityConstraints.MaxDescriptionLength);

        builder.Property(x => x.Cost)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false)
            .ValueGeneratedOnAddOrUpdate();

        // Profile relationship (One-to-Many)
        builder.HasOne(j => j.Author)
            .WithMany(p => p.JobPostings)
            .HasForeignKey(j => j.AuthorId)
            .IsRequired();

        // Area relationship (One-to-Many)
        builder.HasOne(j => j.Area)
            .WithMany(a => a.JobPostings)
            .HasForeignKey(j => j.AreaId)
            .IsRequired();

        // JobCategory relationship (One-to-Many)
        builder.HasOne(j => j.Category)
            .WithMany(c => c.JobPostings)
            .HasForeignKey(j => j.CategoryId)
            .IsRequired();

        // Job relationship (One-to-One)
        builder.HasOne(jp => jp.Job)
            .WithOne(j => j.JobPosting)
            .HasForeignKey<Jobs.Job>(j => j.JobPostingId)
            .IsRequired(false);

        // Configure one-to-many relationship with JobPostingMedia
        builder.HasMany(jp => jp.Medias)
            .WithOne(m => m.JobPosting)
            .HasForeignKey(m => m.JobPostingId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false)
            ;
    }
}
