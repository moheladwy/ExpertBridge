using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Entities.Tags;

public class TagEntityConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.EnglishName)
            .IsRequired()
            .HasMaxLength(TagEntityConstraints.MaxNameLength);

        builder.HasIndex(x => x.EnglishName).IsUnique();
        builder.HasIndex(x => x.ArabicName).IsUnique();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(TagEntityConstraints.MaxDescriptionLength);
    }
}
