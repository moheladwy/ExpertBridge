using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Media.PostMedia;

public class PostMediaEntityConfiguration : IEntityTypeConfiguration<PostMedia>
{
    public void Configure(EntityTypeBuilder<PostMedia> builder)
    {
        builder.HasKey(pm => pm.Id);

        builder.Property(pm => pm.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        // Post relationship (One-to-Many)
        builder.HasOne(pm => pm.Post)
            .WithMany(post => post.Medias)
            .HasForeignKey(pm => pm.PostId)
            .IsRequired();

        // Media relationship (One-to-one)
        builder.HasOne(pm => pm.Media)
            .WithOne(media => media.Post)
            .HasForeignKey<PostMedia>(pm => pm.MediaId)
            .IsRequired();

        // MediaId index (Unique),
        // because one media can only belong to one post.
        // But one post can have many media (One-to-Many).
        // So, that's why we need to make MediaId unique.
        // But PostId can be the same for many PostMedia.
        builder.HasIndex(pm => pm.MediaId).IsUnique();
    }
}
