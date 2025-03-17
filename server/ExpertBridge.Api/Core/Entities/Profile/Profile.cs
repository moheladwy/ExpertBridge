using System.Text.Json.Serialization;
using ExpertBridge.Api.Core.Entities.Chat.ChatParticipant;
using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ProfileBadge;
using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ProfileSkill;
using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ProfileTag;
using ExpertBridge.Api.Core.Entities.Media.ProfileMedia;
using ExpertBridge.Api.Core.Entities.Votes.CommentVote;
using ExpertBridge.Api.Core.Entities.Votes.PostVote;

namespace ExpertBridge.Api.Core.Entities.Profile;

public partial class Profile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string UserId { get; set; }
    public string? JobTitle { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public double Rating { get; set; }
    public int RatingCount { get; set; }
}

public partial class Profile
{
    // Navigation properties
    [JsonIgnore]
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
