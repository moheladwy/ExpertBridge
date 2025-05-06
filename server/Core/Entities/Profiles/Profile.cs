using System.Text.Json.Serialization;
using Core.Entities.CommentVotes;
using Core.Entities.ManyToManyRelationships.ChatParticipants;
using Core.Entities.ManyToManyRelationships.ProfileBadges;
using Core.Entities.ManyToManyRelationships.ProfileSkills;
using Core.Entities.ManyToManyRelationships.UserInterests;
using Core.Entities.Media.ProfileMedia;
using Core.Entities.PostVotes;
using Pgvector;

namespace Core.Entities.Profiles;

public partial class Profile : BaseModel, ISoftDeletable
{
    public string UserId { get; set; }
    public string? JobTitle { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public double Rating { get; set; }
    public int RatingCount { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; }
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsBanned { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Vector? UserInterestEmbedding { get; set; }
}

public partial class Profile
{
    // Navigation properties
    [JsonIgnore]
    public Users.User User { get; set; }
    public ICollection<Areas.Area> Areas { get; set; } = [];
    public ICollection<ProfileExperiences.ProfileExperience> Experiences { get; set; } = [];
    public ICollection<ProfileMedia> Medias { get; set; } = [];

    [JsonIgnore]
    public ICollection<Posts.Post> Posts { get; set; } = [];
    [JsonIgnore]
    public ICollection<Comments.Comment> Comments { get; set; } = [];
    public ICollection<JobPostings.JobPosting> JobPostings { get; set; } = [];
    public ICollection<Jobs.Job> JobsAsAuthor { get; set; } = [];
    public ICollection<Jobs.Job> JobsAsWorker { get; set; } = [];
    public ChatParticipant ChatParticipant { get; set; }
    public ICollection<ProfileSkill> ProfileSkills { get; set; } = [];
    public ICollection<UserInterest> ProfileTags { get; set; } = [];
    public ICollection<ProfileBadge> ProfileBadges { get; set; } = [];
    public ICollection<PostVote> PostVotes { get; set; } = [];
    public ICollection<CommentVote> CommentVotes { get; set; } = [];
}
