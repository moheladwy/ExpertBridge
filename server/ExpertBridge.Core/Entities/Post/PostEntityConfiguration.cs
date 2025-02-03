using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Post;

public class PostEntityConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(PostEntityConstraints.MaxTitleLength);

        builder.HasIndex(x => x.Title);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(PostEntityConstraints.MaxContentLength);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.LastModified)
            .IsRequired(false)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.isDeleted)
            .IsRequired();

        // Configure one-to-many relationship
        builder.HasOne(p => p.Author)
            .WithMany(p => p.Posts)
            .HasForeignKey(p => p.AuthorId)
            .IsRequired();
    }
}
