using ExpertBridge.Core.EntityConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Posts;

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
        builder.HasIndex(x => x.IsProcessed);
        builder.HasIndex(x => x.IsTagged);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(PostEntityConstraints.MaxContentLength);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.LastModified)
            .IsRequired(false)
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        // Configure one-to-many relationship
        //builder.HasOne(p => p.Author)
        //    .WithMany(p => p.Posts)
        //    .HasForeignKey(p => p.AuthorId)
        //    .IsRequired();

        // Configure one-to-many relationship with PostMedia
        builder.HasMany(p => p.Medias)
            .WithOne(m => m.Post)
            .HasForeignKey(pm => pm.PostId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.Property(p => p.Embedding)
            .HasColumnType(ColumnTypes.Vector1024)
            .IsRequired(false);

        builder
            .HasIndex(p => p.Embedding)
            .HasMethod(IndexMethods.Hnsw)
            .HasOperators("vector_cosine_ops")
            .HasStorageParameter("m", 64) // Example, tune this
            .HasStorageParameter("ef_construction", 128) // Example, tune this
            ;
    }
}
