using System.Linq.Expressions;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.Areas;
using ExpertBridge.Core.Entities.Badges;
using ExpertBridge.Core.Entities.Chats;
using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.CommentVotes;
using ExpertBridge.Core.Entities.JobCategories;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.JobReviews;
using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.JobStatuses;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ChatParticipants;
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
using ExpertBridge.Core.Entities.ModerationReports;
using ExpertBridge.Core.Entities.Notifications;
using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Entities.PostVotes;
using ExpertBridge.Core.Entities.ProfileExperiences;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Skills;
using ExpertBridge.Core.Entities.Tags;
using ExpertBridge.Core.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace ExpertBridge.Data.DatabaseContexts;

public sealed class ExpertBridgeDbContext : DbContext
{
    public ExpertBridgeDbContext(DbContextOptions<ExpertBridgeDbContext> options)
        : base(options)
    {
        ChangeTracker.Tracked += UpdateTimestamps;
        ChangeTracker.StateChanged += UpdateTimestamps;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<ProfileExperience> ProfileExperiences { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobReview> JobReviews { get; set; }
    public DbSet<JobStatus> JobStatuses { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<PostVote> PostVotes { get; set; }
    public DbSet<CommentVote> CommentVotes { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<JobCategory> JobCategories { get; set; }
    public DbSet<JobPosting> JobPostings { get; set; }
    public DbSet<ProfileBadge> ProfileBadges { get; set; }
    public DbSet<UserInterest> UserInterests { get; set; }
    public DbSet<ProfileSkill> ProfileSkills { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatParticipant> ChatParticipants { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<ChatMedia> ChatMedias { get; set; }
    public DbSet<ProfileMedia> ProfileMedias { get; set; }
    public DbSet<ProfileExperienceMedia> ProfileExperienceMedias { get; set; }
    public DbSet<CommentMedia> CommentMedias { get; set; }
    public DbSet<JobPostingMedia> JobPostingMedias { get; set; }
    public DbSet<PostMedia> PostMedias { get; set; }
    public DbSet<MediaGrant> MediaGrants { get; set; }
    public DbSet<ModerationReport> ModerationReports { get; set; }
    public DbSet<Notification> Notifications { get; set; }


    /// <summary>
    ///     The OnModelCreating method that is called when the model is being created.
    /// </summary>
    /// <param name="modelBuilder">
    ///     The model builder that is used to build the model.
    /// </param>
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
        modelBuilder.ApplyConfiguration(new PostVoteEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobReviewEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobStatusEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PostMediaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ChatMediaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobPostingEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserInterestEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CommentVoteEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobCategoryEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileBadgeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileSkillEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileMediaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CommentMediaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobPostingMediaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ChatParticipantEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileExperienceEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileExperienceMediaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MediaGrantEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ModerationReportEntityConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationEntityConfiguration());
    }

    // Event handlers
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
