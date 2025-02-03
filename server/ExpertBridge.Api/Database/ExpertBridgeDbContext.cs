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
using ExpertBridge.Core.Entities.Media.MediaType;
using ExpertBridge.Core.Entities.Post;
using ExpertBridge.Core.Entities.Profile;
using ExpertBridge.Core.Entities.ProfileExperience;
using ExpertBridge.Core.Entities.Skill;
using ExpertBridge.Core.Entities.Tags;
using ExpertBridge.Core.Entities.User;
using ExpertBridge.Core.Entities.Votes.CommentVote;
using ExpertBridge.Core.Entities.Votes.PostVote;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Database;

internal class ExpertBridgeDbContext(DbContextOptions<ExpertBridgeDbContext> options) : DbContext(options)
{
    DbSet<User> Users { get; set; }
    DbSet<Profile> Profiles { get; set; }
    DbSet<ProfileExperience> ProfileExperiences { get; set; }
    DbSet<Skill> Skills { get; set; }
    DbSet<Tag> Tags { get; set; }
    DbSet<Job> Jobs { get; set; }
    DbSet<JobReview> JobReviews { get; set; }
    DbSet<JobStatus> JobStatuses { get; set; }
    DbSet<Post> Posts { get; set; }
    DbSet<Comment> Comments { get; set; }
    DbSet<PostVote> PostVotes { get; set; }
    DbSet<CommentVote> CommentVotes { get; set; }
    DbSet<Area> Areas { get; set; }
    DbSet<Badge> Badges { get; set; }
    DbSet<JobCategory> JobCategories { get; set; }
    DbSet<JobPosting> JobPostings { get; set; }
    DbSet<Media> Media { get; set; }
    DbSet<MediaType> MediaTypes { get; set; }
    DbSet<ProfileBadge> ProfileBadges { get; set; }
    DbSet<ProfileTag> ProfileTags { get; set; }
    DbSet<ProfileSkill> ProfileSkills { get; set; }
    DbSet<Chat> Chats { get; set; }
    DbSet<ChatParticipant> ChatParticipants { get; set; }
    DbSet<PostTag> PostTags { get; set; }

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
        modelBuilder.ApplyConfiguration(new JobPostingEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileTagEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CommentVoteEntityConfiguration());
        modelBuilder.ApplyConfiguration(new JobCategoryEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileBadgeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileSkillEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ChatParticipantEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileExperienceEntityConfiguration());
    }
}
