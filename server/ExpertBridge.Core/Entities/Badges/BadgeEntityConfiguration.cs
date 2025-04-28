

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Badges;

public class BadgeEntityConfiguration : IEntityTypeConfiguration<Badge>
{
    public void Configure(EntityTypeBuilder<Badge> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(BadgeEntityConstraints.MaxNameLength);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(BadgeEntityConstraints.MaxDescriptionLength);
    }
}
