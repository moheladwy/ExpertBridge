using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Media.MediaType;

public class MediaTypeEntityConfiguration : IEntityTypeConfiguration<MediaType>
{
    public void Configure(EntityTypeBuilder<MediaType> builder)
    {
        builder.HasKey(mediaType => mediaType.Id);

        builder.Property(mediaType => mediaType.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(mediaType => mediaType.Type)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<MediaTypeEnum>(v)
            )
            .HasMaxLength(GlobalEntitiesConstraints.MaxEnumsLength);
    }
}
