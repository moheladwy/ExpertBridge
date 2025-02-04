using ExpertBridge.Core.Entities.Area;
using ExpertBridge.Core.Entities.Badge;
using ExpertBridge.Core.Entities.Chat;
using ExpertBridge.Core.Entities.Chat.ChatParticipant;
using ExpertBridge.Core.Entities.Comment;
using ExpertBridge.Core.Entities.Job;
using ExpertBridge.Core.Entities.Job.JobReview;
using ExpertBridge.Core.Entities.Job.JobStatus;
using ExpertBridge.Core.Entities.JobCategory;
using ExpertBridge.Core.Entities.JobPosting;
using ExpertBridge.Core.Entities.ManyToManyRelationships.PostTag;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileBadge;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkill;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileTag;
using ExpertBridge.Core.Entities.Media;
using ExpertBridge.Core.Entities.Media.ChatMedia;
using ExpertBridge.Core.Entities.Media.CommentMedia;
using ExpertBridge.Core.Entities.Media.JobPostingMedia;
using ExpertBridge.Core.Entities.Media.MediaType;
using ExpertBridge.Core.Entities.Media.PostMedia;
using ExpertBridge.Core.Entities.Media.ProfileExperienceMedia;
using ExpertBridge.Core.Entities.Media.ProfileMedia;
using ExpertBridge.Core.Entities.Post;
using ExpertBridge.Core.Entities.Profile;
using ExpertBridge.Core.Entities.ProfileExperience;
using ExpertBridge.Core.Entities.Skill;
using ExpertBridge.Core.Entities.Tags;
using ExpertBridge.Core.Entities.User;
using ExpertBridge.Core.Entities.Votes.CommentVote;
using ExpertBridge.Core.Entities.Votes.PostVote;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Data.DatabaseContexts;

public sealed class ExpertBridgeDbContext(DbContextOptions<ExpertBridgeDbContext> options) : DbContext(options)
{
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
    public DbSet<Media> Media { get; set; }
    public DbSet<MediaType> MediaTypes { get; set; }
    public DbSet<ProfileBadge> ProfileBadges { get; set; }
    public DbSet<ProfileTag> ProfileTags { get; set; }
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


    /// <summary>
    ///     The OnModelCreating method that is called when the model is being created.
    /// </summary>
    /// <param name="modelBuilder">
    ///     The model builder that is used to build the model.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new TagEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PostEntityConfiguration());
        modelBuilder.ApplyConfiguration(new AreaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ChatEntityConfiguration());
        modelBuilder.ApplyConfiguration(new SkillEntityConfiguration());
        modelBuilder.ApplyConfiguration(new BadgeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MediaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PostTagEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CommentEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PostVoteEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobReviewEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MediaTypeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobStatusEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PostMediaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ChatMediaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobPostingEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileTagEntityConfiguration());
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
    }
}
