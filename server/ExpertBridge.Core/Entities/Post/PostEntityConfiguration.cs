using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Post;

public class PostEntityConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(PostEntityConstraints.MaxTitleLength);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(PostEntityConstraints.MaxContentLength);

        // TODO: Add AuthorId FK.

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.LastModified)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.isDeleted)
            .IsRequired();
    }
}
