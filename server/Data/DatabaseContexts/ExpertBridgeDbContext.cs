using System.Linq.Expressions;
using Core.Entities;
using Core.Entities.Areas;
using Core.Entities.Badges;
using Core.Entities.Chats;
using Core.Entities.Comments;
using Core.Entities.CommentVotes;
using Core.Entities.JobCategories;
using Core.Entities.JobPostings;
using Core.Entities.JobReviews;
using Core.Entities.Jobs;
using Core.Entities.JobStatuses;
using Core.Entities.ManyToManyRelationships.ChatParticipants;
using Core.Entities.ManyToManyRelationships.PostTags;
using Core.Entities.ManyToManyRelationships.ProfileBadges;
using Core.Entities.ManyToManyRelationships.ProfileSkills;
using Core.Entities.ManyToManyRelationships.UserInterests;
using Core.Entities.Media.ChatMedia;
using Core.Entities.Media.CommentMedia;
using Core.Entities.Media.JobPostingMedia;
using Core.Entities.Media.MediaGrants;
using Core.Entities.Media.PostMedia;
using Core.Entities.Media.ProfileExperienceMedia;
using Core.Entities.Media.ProfileMedia;
using Core.Entities.ModerationReports;
using Core.Entities.Notifications;
using Core.Entities.Posts;
using Core.Entities.PostVotes;
using Core.Entities.ProfileExperiences;
using Core.Entities.Profiles;
using Core.Entities.Skills;
using Core.Entities.Tags;
using Core.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Data.DatabaseContexts;

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
