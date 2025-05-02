using ExpertBridge.Core.Entities.Media.CommentMedia;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Comments;

public class CommentEntityConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(CommentEntityConstraints.MaxContentLength);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.LastModified)
            .IsRequired(false)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        // Configure one-to-many relationship with Profile
        builder.HasOne(c => c.Author)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.AuthorId)
            .IsRequired();

        // Configure one-to-many relationship with Post
        builder.HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .IsRequired();

        // Configure one-to-many relationship with Comment (Parent comment)
        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .IsRequired(false);

        // Configure self-referencing relationship with cascade delete for replies
        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);  // Parent won't be deleted

        // Configure cascade delete for replies when parent is deleted
        builder.HasMany(c => c.Replies)
            .WithOne(c => c.ParentComment)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-one relationship with CommentMedia
        builder.HasMany(c => c.Medias)
            .WithOne(m => m.Comment)
            .HasForeignKey(m => m.CommentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade)
            ;
    }
}
