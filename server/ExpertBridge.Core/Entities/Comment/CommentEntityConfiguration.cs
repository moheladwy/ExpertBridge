using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Comment;

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

        builder.Property(x => x.isDeleted)
            .IsRequired();

        // Configure one-to-many relationship with Profile
        builder.HasOne(c => c.Author)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.AuthorId)
            .IsRequired();

        // Configure one-to-many relationship with Comment (Parent comment)
        builder.HasOne(c => c.Parent)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentId)
            .IsRequired(false);

        // Configure self-referencing relationship with cascade delete for replies
        builder.HasOne(c => c.Parent)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);  // Parent won't be deleted

        // Configure cascade delete for replies when parent is deleted
        builder.HasMany(c => c.Replies)
            .WithOne(c => c.Parent)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
