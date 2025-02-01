using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Badge;

public class BadgeEntityConfiguration : IEntityTypeConfiguration<Badge>
{
    public void Configure(EntityTypeBuilder<Badge> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(BadgeEntityConstraints.MaxNameLength);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(BadgeEntityConstraints.MaxDescriptionLength);
    }
}
