// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using ExpertBridge.Core.Entities.Areas;
using ExpertBridge.Core.Entities.Chats;
using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.CommentVotes;
using ExpertBridge.Core.Entities.JobApplications;
using ExpertBridge.Core.Entities.JobOffers;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.JobPostingsVotes;
using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ChatParticipants;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileBadges;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkills;
using ExpertBridge.Core.Entities.ManyToManyRelationships.UserInterests;
using ExpertBridge.Core.Entities.Media.ProfileMedia;
using ExpertBridge.Core.Entities.Messages;
using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Entities.PostVotes;
using ExpertBridge.Core.Entities.ProfileExperiences;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Core.Interfaces;
using Pgvector;

namespace ExpertBridge.Core.Entities.Profiles;

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
    public required string Email { get; set; }
    public required string Username { get; set; }
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
    public User User { get; set; }
    public ICollection<Area> Areas { get; set; } = [];
    public ICollection<ProfileExperience> Experiences { get; set; } = [];
    public ICollection<ProfileMedia> Medias { get; set; } = [];

    [JsonIgnore]
    public ICollection<Post> Posts { get; set; } = [];
    [JsonIgnore]
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<JobPosting> JobPostings { get; set; } = [];
    public ICollection<JobApplication> JobApplications { get; set; } = [];
    public ChatParticipant ChatParticipant { get; set; }
    public ICollection<ProfileSkill> ProfileSkills { get; set; } = [];
    public ICollection<UserInterest> ProfileTags { get; set; } = [];
    public ICollection<ProfileBadge> ProfileBadges { get; set; } = [];
    public ICollection<PostVote> PostVotes { get; set; } = [];
    public ICollection<CommentVote> CommentVotes { get; set; } = [];
    public ICollection<JobPostingVote> JobPostingVotes { get; set; } = [];


    public ICollection<Job> JobsAsAuthor { get; set; } = [];
    public ICollection<Job> JobsAsWorker { get; set; } = [];
    public ICollection<JobOffer> AuthoredJobOffers { get; set; } = [];
    public ICollection<JobOffer> ReceivedJobOffers { get; set; } = [];

    public ICollection<Chat> ChatsAsWorker { get; set; } = [];
    public ICollection<Chat> ChatsAsHirer { get; set; } = [];

    public ICollection<Message> SentMessages { get; set; } = [];
}
