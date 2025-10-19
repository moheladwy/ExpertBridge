// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.JobPostingsVotes;

/// <summary>
/// Configures the entity mapping for the <see cref="JobPostingVote"/> entity.
/// Defines the database schema for job posting votes including primary key, timestamps, relationships with profiles and job postings, and the upvote/downvote flag.
/// </summary>
public class JobPostingVoteEntityConfiguration : IEntityTypeConfiguration<JobPostingVote>
{
    /// <summary>
    /// Configures the entity type for JobPostingVote, including primary key, vote type property, relationships with Profile and JobPosting entities, and cascade delete behaviors.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the JobPostingVote entity.</param>
    public void Configure(EntityTypeBuilder<JobPostingVote> builder)
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
            .WithMany(p => p.JobPostingVotes)
            .HasForeignKey(v => v.ProfileId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with JobPosting
        builder.HasOne(v => v.JobPosting)
            .WithMany(p => p.Votes)
            .HasForeignKey(v => v.JobPostingId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the unique constraint
        builder.HasIndex(x => new { x.ProfileId, x.JobPostingId })
            .IsUnique();
    }
}
