using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Media;

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

        // MediaType relationship (One-to-Many)
        builder.HasOne(m => m.MediaType)
            .WithMany(mt => mt.Medias)
            .HasForeignKey(m => m.MediaTypeId)
            .IsRequired();
    }
}
