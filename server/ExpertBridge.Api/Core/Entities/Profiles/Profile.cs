// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ChatParticipants;
using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ProfileBadges;
using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ProfileSkills;
using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ProfileTags;
using ExpertBridge.Api.Core.Entities.Media.ProfileMedia;
using ExpertBridge.Api.Core.Entities.Votes.CommentVote;
using ExpertBridge.Api.Core.Entities.Votes.PostVote;

namespace ExpertBridge.Api.Core.Entities.Profiles;

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
    public Users.User User { get; set; }
    public ICollection<Areas.Area> Areas { get; set; } = [];
    public ICollection<ProfileExperiences.ProfileExperience> Experiences { get; set; } = [];
    public ICollection<ProfileMedia> Medias { get; set; } = [];
    public ICollection<Posts.Post> Posts { get; set; } = [];
    public ICollection<Comments.Comment> Comments { get; set; } = [];
    public ICollection<JobPostings.JobPosting> JobPostings { get; set; } = [];
    public ICollection<Jobs.Job> JobsAsAuthor { get; set; } = [];
    public ICollection<Jobs.Job> JobsAsWorker { get; set; } = [];
    public ChatParticipant ChatParticipant { get; set; }
    public ICollection<ProfileSkill> ProfileSkills { get; set; } = [];
    public ICollection<ProfileTag> ProfileTags { get; set; } = [];
    public ICollection<ProfileBadge> ProfileBadges { get; set; } = [];
    public ICollection<PostVote> PostVotes { get; set; } = [];
    public ICollection<CommentVote> CommentVotes { get; set; } = [];
}
