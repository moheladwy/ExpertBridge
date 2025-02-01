using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Media.MediaType;

public class MediaTypeEntityConfiguration : IEntityTypeConfiguration<MediaType>
{
    public void Configure(EntityTypeBuilder<MediaType> builder)
    {
        builder.HasKey(mediaType => mediaType.Id);

        builder.Property(mediaType => mediaType.Id)
            .ValueGeneratedOnAdd();

        builder.Property(mediaType => mediaType.Type)
            .IsRequired()
            .HasConversion<string>();
    }
}
