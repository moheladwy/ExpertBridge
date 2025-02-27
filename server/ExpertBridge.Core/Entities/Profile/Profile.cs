using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileBadge;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkill;
using ExpertBridge.Core.Entities.Chat.ChatParticipant;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileTag;
using ExpertBridge.Core.Entities.Votes.CommentVote;
using ExpertBridge.Core.Entities.Votes.PostVote;
using ExpertBridge.Core.Entities.Media.ProfileMedia;

namespace ExpertBridge.Core.Entities.Profile;

public partial class Profile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string UserId { get; set; }
    public string? JobTitle { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public int Rating { get; set; }
    public int RatingCount { get; set; }
}

public partial class Profile
{
    // Navigation properties
    public User.User User { get; set; }
    public ICollection<Area.Area> Areas { get; set; } = [];
    public ICollection<ProfileExperience.ProfileExperience> Experiences { get; set; } = [];
    public ICollection<ProfileMedia> Medias { get; set; } = [];
    public ICollection<Post.Post> Posts { get; set; } = [];
    public ICollection<Comment.Comment> Comments { get; set; } = [];
    public ICollection<JobPosting.JobPosting> JobPostings { get; set; } = [];
    public ICollection<Job.Job> JobsAsAuthor { get; set; } = [];
    public ICollection<Job.Job> JobsAsWorker { get; set; } = [];
    public ChatParticipant ChatParticipant { get; set; }
    public ICollection<ProfileSkill> ProfileSkills { get; set; } = [];
    public ICollection<ProfileTag> ProfileTags { get; set; } = [];
    public ICollection<ProfileBadge> ProfileBadges { get; set; } = [];
    public ICollection<PostVote> PostVotes { get; set; } = [];
    public ICollection<CommentVote> CommentVotes { get; set; } = [];
}
