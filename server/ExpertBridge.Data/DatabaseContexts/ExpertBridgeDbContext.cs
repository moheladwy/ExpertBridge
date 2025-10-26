// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Core.Entities.Areas;
using ExpertBridge.Core.Entities.Badges;
using ExpertBridge.Core.Entities.Chats;
using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.CommentVotes;
using ExpertBridge.Core.Entities.JobApplications;
using ExpertBridge.Core.Entities.JobCategories;
using ExpertBridge.Core.Entities.JobOffers;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.JobPostingsVotes;
using ExpertBridge.Core.Entities.JobReviews;
using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ChatParticipants;
using ExpertBridge.Core.Entities.ManyToManyRelationships.JobPostingTags;
using ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileBadges;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkills;
using ExpertBridge.Core.Entities.ManyToManyRelationships.UserInterests;
using ExpertBridge.Core.Entities.Media.ChatMedia;
using ExpertBridge.Core.Entities.Media.CommentMedia;
using ExpertBridge.Core.Entities.Media.JobPostingMedia;
using ExpertBridge.Core.Entities.Media.MediaGrants;
using ExpertBridge.Core.Entities.Media.PostMedia;
using ExpertBridge.Core.Entities.Media.ProfileExperienceMedia;
using ExpertBridge.Core.Entities.Media.ProfileMedia;
using ExpertBridge.Core.Entities.Messages;
using ExpertBridge.Core.Entities.ModerationReports;
using ExpertBridge.Core.Entities.Notifications;
using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Entities.PostVotes;
using ExpertBridge.Core.Entities.ProfileExperiences;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Skills;
using ExpertBridge.Core.Entities.Tags;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ExpertBridge.Data.DatabaseContexts;

/// <summary>
///     Represents the Entity Framework Core database context for the ExpertBridge application.
///     Provides access to all entity sets (DbSets) and configures automatic timestamp updates, soft delete query filters,
///     and entity configurations.
///     Supports PostgreSQL with pgvector extension for semantic search capabilities using 1024-dimensional vector
///     embeddings.
/// </summary>
/// <remarks>
///     This context manages the complete ExpertBridge domain model including:
///     - User authentication and profiles with AI-generated interest embeddings
///     - Social features (Posts, Comments, Votes) with AI-powered tagging via Groq API
///     - Freelance marketplace (JobPostings, Jobs, JobApplications, JobOffers) with semantic job matching
///     - Real-time communication (Chats, Messages, Notifications) via SignalR
///     - Media management with AWS S3 integration and presigned URL access control
///     - Skills, badges, and professional experience tracking
///     - Many-to-many relationships for tags, skills, badges, and user interests
///     - Moderation system with AI-assisted content review
///     The context automatically:
///     - Updates CreatedAt timestamps on entity insertion
///     - Updates LastModified timestamps on entity modification
///     - Applies global query filters to exclude soft-deleted entities (IsDeleted = false)
///     - Registers all entity configurations defined in ExpertBridge.Core.EntityConfiguration classes
/// </remarks>
public sealed class ExpertBridgeDbContext : DbContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ExpertBridgeDbContext" /> class with the specified options.
    ///     Configures change tracker event handlers for automatic timestamp management on entity tracking and state changes.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext, including connection string and provider settings.</param>
    public ExpertBridgeDbContext(DbContextOptions<ExpertBridgeDbContext> options)
        : base(options)
    {
        ChangeTracker.Tracked += UpdateTimestamps;
        ChangeTracker.StateChanged += UpdateTimestamps;
    }

    /// <summary>
    ///     Gets or sets the DbSet for User entities representing authenticated users with Firebase identity integration.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for Profile entities representing professional profiles with vector embeddings for
    ///     personalized recommendations.
    /// </summary>
    public DbSet<Profile> Profiles { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for ProfileExperience entities representing work experience, certifications, and education
    ///     entries on professional profiles.
    /// </summary>
    public DbSet<ProfileExperience> ProfileExperiences { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for Skill entities representing technical and professional skills available in the platform.
    /// </summary>
    public DbSet<Skill> Skills { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for Tag entities representing AI-generated multilingual content tags used for categorization
    ///     and discovery.
    /// </summary>
    public DbSet<Tag> Tags { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for Job entities representing completed freelance work contracts between experts and
    ///     clients.
    /// </summary>
    public DbSet<Job> Jobs { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for JobReview entities representing ratings and feedback on completed jobs.
    /// </summary>
    public DbSet<JobReview> JobReviews { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for Post entities representing social posts with AI-powered tagging and semantic search
    ///     capabilities.
    /// </summary>
    public DbSet<Post> Posts { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for Comment entities representing threaded discussions on posts and job postings.
    /// </summary>
    public DbSet<Comment> Comments { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for PostVote entities representing upvotes and downvotes on posts.
    /// </summary>
    public DbSet<PostVote> PostVotes { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for CommentVote entities representing upvotes and downvotes on comments.
    /// </summary>
    public DbSet<CommentVote> CommentVotes { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for JobPosting entities representing freelance job opportunities with AI-powered semantic
    ///     matching using pgvector.
    /// </summary>
    public DbSet<JobPosting> JobPostings { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for JobPostingTag entities representing the many-to-many relationship between job postings
    ///     and tags.
    /// </summary>
    public DbSet<JobPostingTag> JobPostingTags { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for JobPostingVote entities representing upvotes and downvotes on job postings.
    /// </summary>
    public DbSet<JobPostingVote> JobPostingVotes { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for Area entities representing geographic regions and locations for job postings.
    /// </summary>
    public DbSet<Area> Areas { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for Badge entities representing professional achievements and credentials earned by experts.
    /// </summary>
    public DbSet<Badge> Badges { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for JobCategory entities representing hierarchical categorization of job types and
    ///     industries.
    /// </summary>
    public DbSet<JobCategory> JobCategories { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for JobApplication entities representing expert applications to job postings with proposal
    ///     details.
    /// </summary>
    public DbSet<JobApplication> JobApplications { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for ProfileBadge entities representing the many-to-many relationship between profiles and
    ///     earned badges.
    /// </summary>
    public DbSet<ProfileBadge> ProfileBadges { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for UserInterest entities representing the many-to-many relationship between users and their
    ///     topic interests for personalized content recommendations.
    /// </summary>
    public DbSet<UserInterest> UserInterests { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for ProfileSkill entities representing the many-to-many relationship between profiles and
    ///     their professional skills.
    /// </summary>
    public DbSet<ProfileSkill> ProfileSkills { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for Chat entities representing real-time conversation channels between platform users.
    /// </summary>
    public DbSet<Chat> Chats { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for ChatParticipant entities representing the many-to-many relationship between chats and
    ///     participating profiles.
    /// </summary>
    public DbSet<ChatParticipant> ChatParticipants { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for PostTag entities representing the many-to-many relationship between posts and
    ///     AI-generated tags.
    /// </summary>
    public DbSet<PostTag> PostTags { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for ChatMedia entities representing file attachments shared in chat conversations.
    /// </summary>
    public DbSet<ChatMedia> ChatMedias { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for ProfileMedia entities representing media files associated with professional profiles
    ///     (profile pictures, cover photos, portfolio items).
    /// </summary>
    public DbSet<ProfileMedia> ProfileMedias { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for ProfileExperienceMedia entities representing work samples, certifications, and evidence
    ///     files for profile experience entries.
    /// </summary>
    public DbSet<ProfileExperienceMedia> ProfileExperienceMedias { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for CommentMedia entities representing file attachments included in comments.
    /// </summary>
    public DbSet<CommentMedia> CommentMedias { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for JobPostingMedia entities representing project examples, requirements, and reference
    ///     files for job postings.
    /// </summary>
    public DbSet<JobPostingMedia> JobPostingMedias { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for PostMedia entities representing images, videos, and file attachments in social posts.
    /// </summary>
    public DbSet<PostMedia> PostMedias { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for MediaGrant entities representing temporary access grants for AWS S3 presigned URL
    ///     generation and media upload authorization.
    /// </summary>
    public DbSet<MediaGrant> MediaGrants { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for ModerationReport entities representing content moderation reports with AI-assisted
    ///     review using Groq API.
    /// </summary>
    public DbSet<ModerationReport> ModerationReports { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for Notification entities representing real-time notifications delivered via SignalR to
    ///     platform users.
    /// </summary>
    public DbSet<Notification> Notifications { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for JobOffer entities representing contract offers from clients to experts for job postings.
    /// </summary>
    public DbSet<JobOffer> JobOffers { get; set; }

    /// <summary>
    ///     Gets or sets the DbSet for Message entities representing individual messages within chat conversations with
    ///     real-time delivery via SignalR.
    /// </summary>
    public DbSet<Message> Messages { get; set; }


    /// <summary>
    ///     Configures the database model when it is being created.
    ///     Applies entity configurations, enables PostgreSQL vector extension, and sets up global query filters for
    ///     soft-deleted entities.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure the database model and apply entity configurations.</param>
    /// <remarks>
    ///     This method performs the following configurations:
    ///     - Enables PostgreSQL "vector" extension for pgvector semantic search functionality
    ///     - Applies global query filter (IsDeleted == false) to all entities implementing ISoftDeletable interface
    ///     - Registers all entity configurations from ExpertBridge.Core.EntityConfiguration classes
    ///     - Configures relationships, indexes, constraints, and database mappings for all 35+ entity types
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("vector");

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var condition = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda(condition, parameter);
                entityType.SetQueryFilter(lambda);
            }
        }

        modelBuilder.ApplyConfiguration(new TagEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PostEntityConfiguration());
        modelBuilder.ApplyConfiguration(new AreaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ChatEntityConfiguration());
        modelBuilder.ApplyConfiguration(new SkillEntityConfiguration());
        modelBuilder.ApplyConfiguration(new BadgeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PostTagEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CommentEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MessageEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobOfferEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PostVoteEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobReviewEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PostMediaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ChatMediaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobPostingEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MediaGrantEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CommentVoteEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobCategoryEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileBadgeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserInterestEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileSkillEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileMediaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CommentMediaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobPostingTagEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobPostingVoteEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobApplicationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobPostingMediaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ChatParticipantEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ModerationReportEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileExperienceEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileExperienceMediaEntityConfiguration());
    }

    /// <summary>
    ///     Event handler that automatically updates timestamp properties on entities implementing <see cref="ITimestamped" />
    ///     interface.
    ///     Sets CreatedAt on entity insertion and LastModified on entity modification using UTC time.
    /// </summary>
    /// <param name="sender">The event sender (typically the ChangeTracker).</param>
    /// <param name="e">The entity entry event arguments containing entity state and metadata.</param>
    /// <remarks>
    ///     This handler is attached to both ChangeTracker.Tracked and ChangeTracker.StateChanged events in the constructor.
    ///     It ensures consistent timestamp management across all timestamped entities without manual intervention.
    ///     Console logging is included for debugging timestamp application during development.
    /// </remarks>
    private static void UpdateTimestamps(object? sender, EntityEntryEventArgs e)
    {
        if (e.Entry.Entity is ITimestamped entityWithTimestamps)
        {
            switch (e.Entry.State)
            {
                case EntityState.Modified:
                    entityWithTimestamps.LastModified = DateTime.UtcNow;
                    Console.WriteLine($"Stamped for update: {e.Entry.Entity}");
                    break;
                case EntityState.Added:
                    entityWithTimestamps.CreatedAt = DateTime.UtcNow;
                    Console.WriteLine($"Stamped for insert: {e.Entry.Entity}");
                    break;
            }
        }
    }
}
