using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Media;

public class MediaEntityConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(MediaEntityConstraints.MaxNameLength);

        builder.Property(x => x.MediaUrl)
            .IsRequired()
            .HasMaxLength(MediaEntityConstraints.MaxMediaUrlLength);

        builder.HasIndex(x => x.MediaUrl).IsUnique();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.LastModified)
            .IsRequired(false)
            .ValueGeneratedOnAddOrUpdate();

        // Profile relationship (One-to-Many)
        builder.HasOne(m => m.Profile)
            .WithMany(p => p.Medias)
            .HasForeignKey(m => m.ProfileId)
            .IsRequired(false);

        // Post relationship (One-to-Many)
        builder.HasOne(m => m.Post)
            .WithMany(p => p.Medias)
            .HasForeignKey(m => m.PostId)
            .IsRequired(false);

        // JobPosting relationship (One-to-Many)
        builder.HasOne(m => m.JobPosting)
            .WithMany(jp => jp.Medias)
            .HasForeignKey(m => m.JobPostingId)
            .IsRequired(false);

        // Chat relationship (One-to-Many)
        builder.HasOne(m => m.Chat)
            .WithMany(c => c.Medias)
            .HasForeignKey(m => m.ChatId)
            .IsRequired(false);

        // ProfileExperience relationship (One-to-Many)
        builder.HasOne(m => m.ProfileExperience)
            .WithMany(pe => pe.Medias)
            .HasForeignKey(m => m.ProfileExperienceId)
            .IsRequired(false);

        // Comment relationship (One-to-One)
        builder.HasOne(m => m.Comment)
            .WithOne(c => c.Media)
            .HasForeignKey<Media>(m => m.CommentId)
            .IsRequired(false);

        // MediaType relationship (One-to-Many)
        builder.HasOne(m => m.MediaType)
            .WithMany(mt => mt.Medias)
            .HasForeignKey(m => m.MediaTypeId)
            .IsRequired();
    }
}
