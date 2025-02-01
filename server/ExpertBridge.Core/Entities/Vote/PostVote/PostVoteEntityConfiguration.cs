using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Vote.PostVote;

public class PostVoteEntityConfiguration : IEntityTypeConfiguration<PostVote>
{
    public void Configure(EntityTypeBuilder<PostVote> builder)
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

        // Configure relationship with Post
        builder.HasOne(v => v.Post)
            .WithMany()
            .HasForeignKey(v => v.PostId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the unique constraint
        builder.HasIndex(x => new { x.ProfileId, x.PostId })
            .IsUnique();
    }
}
