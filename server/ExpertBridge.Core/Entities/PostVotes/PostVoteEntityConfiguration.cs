// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.PostVotes;

public class PostVoteEntityConfiguration : IEntityTypeConfiguration<PostVote>
{
    public void Configure(EntityTypeBuilder<PostVote> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.IsUpvote)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();

        // Configure relationship with Profile
        builder.HasOne(v => v.Profile)
            .WithMany(p => p.PostVotes)
            .HasForeignKey(v => v.ProfileId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with Post
        builder.HasOne(v => v.Post)
            .WithMany(p => p.Votes)
            .HasForeignKey(v => v.PostId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the unique constraint
        builder.HasIndex(x => new { x.ProfileId, x.PostId })
            .IsUnique();
    }
}
