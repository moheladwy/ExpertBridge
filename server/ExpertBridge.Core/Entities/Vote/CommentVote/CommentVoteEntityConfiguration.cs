using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Vote.CommentVote;

public class CommentVoteEntityConfiguration : IEntityTypeConfiguration<CommentVote>
{
    public void Configure(EntityTypeBuilder<CommentVote> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.IsUpvote)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        // Configure relationship with Profile
        builder.HasOne(v => v.Profile)
            .WithMany()
            .HasForeignKey(v => v.ProfileId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with Comment
        builder.HasOne(v => v.Comment)
            .WithMany()
            .HasForeignKey(v => v.CommentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the unique constraint
        builder.HasIndex(x => new { x.ProfileId, x.CommentId })
            .IsUnique();
    }
}
