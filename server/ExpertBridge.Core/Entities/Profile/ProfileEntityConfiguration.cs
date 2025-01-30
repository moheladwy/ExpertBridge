using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Profile;

public class ProfileEntityConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.JobTitle)
            .IsRequired()
            .HasMaxLength(ProfileEntityConstraints.JobTitleMaxLength);

        builder.Property(x => x.Bio)
            .IsRequired()
            .HasMaxLength(ProfileEntityConstraints.BioMaxLength);

        builder.Property(x => x.Rating)
            .IsRequired();

        builder.HasOne(p => p.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<User.User>(u => u.Id);

        builder.HasOne(p => p.Area)
            .WithOne(a => a.Profile)
            .HasForeignKey<Area.Area>(a => a.Id);
    }
}
