using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.ProfileExperience;

public class ProfileExperienceConfiguration : IEntityTypeConfiguration<ProfileExperience>
{
    public void Configure(EntityTypeBuilder<ProfileExperience> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(ProfileExperienceConstraints.MaxTitleLength);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(ProfileExperienceConstraints.MaxDescriptionLength);

        builder.Property(x => x.Company)
            .IsRequired()
            .HasMaxLength(ProfileExperienceConstraints.MaxCompanyLength);

        builder.Property(x => x.Location)
            .IsRequired()
            .HasMaxLength(ProfileExperienceConstraints.MaxLocationLength);

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.HasOne(x => x.Profile)
            .WithMany(x => x.Experiences)
            .HasForeignKey("ProfileId");

    }
}
