using ExpertBridge.Core.Entities.Media;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Core.EntityConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Profiles;

public class ProfileEntityConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(UserEntityConstraints.MaxEmailLength);

        builder.HasIndex(x => x.Email)
            .HasFilter(IndexFilters.NotDeleted)
            .IsUnique();

        builder.Property(x => x.JobTitle)
            .IsRequired(false)
            .HasMaxLength(ProfileEntityConstraints.JobTitleMaxLength);

        builder.Property(x => x.Bio)
            .IsRequired(false)
            .HasMaxLength(ProfileEntityConstraints.BioMaxLength);

        builder.Property(x => x.ProfilePictureUrl)
            .IsRequired(false)
            .HasMaxLength(MediaEntityConstraints.MaxMediaUrlLength);

        builder.Property(x => x.Rating)
            .IsRequired();

        builder.Property(x => x.RatingCount)
            .IsRequired();

        builder.Property(x => x.Username)
            .IsRequired(false)
            .HasMaxLength(UserEntityConstraints.MaxUsernameLength);

        builder.HasIndex(x => x.Username)
            .HasFilter(IndexFilters.NotDeleted)
            .IsUnique();

        builder.HasOne(p => p.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<Profile>(p => p.UserId)
            .IsRequired();

        builder.HasMany(p => p.Posts)
            .WithOne(p => p.Author)
            .HasForeignKey(p => p.AuthorId)
            .IsRequired();

        builder.HasMany(p => p.Medias)
            .WithOne(m => m.Profile)
            .HasForeignKey(m => m.ProfileId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false)
            ;

        builder.Property(x => x.UserInterestEmbedding)
            .HasColumnType(ColumnTypes.Vector1024)
            .IsRequired(false)
            ;
    }
}
