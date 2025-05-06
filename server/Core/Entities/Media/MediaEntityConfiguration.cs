using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Entities.Media;

public static class MediaEntityConfiguration
{
    public static void ConfigureAbstractMedia<TEntity>(
        this EntityTypeBuilder<TEntity> builder) where TEntity : MediaObject
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(MediaEntityConstraints.MaxNameLength);

        builder.Property(x => x.Key)
            .IsRequired();

        builder.HasIndex(x => x.Key)
            .IsUnique();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.LastModified)
            .IsRequired(false)
            .ValueGeneratedOnAddOrUpdate();
    }
}
