// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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

/// <summary>
///     Represents a user's professional profile in the ExpertBridge platform.
/// </summary>
/// <remarks>
///     The Profile entity contains comprehensive professional information including experience, skills,
///     ratings, and relationships to user-generated content. It has a one-to-one relationship with the <see cref="User" />
///     entity.
/// </remarks>
public sealed partial class Profile : BaseModel, ISoftDeletable
{
    /// <summary>
    ///     Gets or sets the unique identifier of the associated user account.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    ///     Gets or sets the user's current job title or professional designation.
    /// </summary>
    public string? JobTitle { get; set; }

    /// <summary>
    ///     Gets or sets the user's professional biography or description.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the user's profile picture.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    ///     Gets or sets the average rating of the user's work and expertise.
    /// </summary>
    public double Rating { get; set; }

    /// <summary>
    ///     Gets or sets the total number of ratings received by the user.
    /// </summary>
    public int RatingCount { get; set; }

    /// <summary>
    ///     Gets or sets the user's first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    ///     Gets or sets the user's last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    ///     Gets or sets the user's email address.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    ///     Gets or sets the user's unique username.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    ///     Gets or sets the user's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user is banned from the platform.
    /// </summary>
    public bool IsBanned { get; set; }

    /// <summary>
    ///     Gets or sets the vector embedding of the user's interests for AI-powered recommendations.
    /// </summary>
    /// <remarks>
    ///     This embedding is generated from the user's interests and activity, enabling personalized content recommendations
    ///     using semantic similarity search with PostgreSQL pgvector.
    /// </remarks>
    public Vector? UserInterestEmbedding { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the profile is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the profile was marked as deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}

/// <summary>
///     Contains navigation properties for the Profile entity.
/// </summary>
public partial class Profile
{
    /// <summary>
    ///     Gets or sets the associated user account.
    /// </summary>
    [JsonIgnore]
    public User User { get; set; }

    /// <summary>
    ///     Gets or sets the collection of geographic areas associated with the profile.
    /// </summary>
    public ICollection<Area> Areas { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of professional experiences.
    /// </summary>
    public ICollection<ProfileExperience> Experiences { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of media attachments for the profile.
    /// </summary>
    public ICollection<ProfileMedia> Medias { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of posts created by the user.
    /// </summary>
    [JsonIgnore]
    public ICollection<Post> Posts { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of comments created by the user.
    /// </summary>
    [JsonIgnore]
    public ICollection<Comment> Comments { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of job postings created by the user.
    /// </summary>
    [JsonIgnore]
    public ICollection<JobPosting> JobPostings { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of job applications submitted by the user.
    /// </summary>
    public ICollection<JobApplication> JobApplications { get; set; } = [];

    /// <summary>
    ///     Gets or sets the chat participant record for this profile.
    /// </summary>
    public ChatParticipant ChatParticipant { get; set; }

    /// <summary>
    ///     Gets or sets the collection of skills associated with the profile.
    /// </summary>
    public ICollection<ProfileSkill> ProfileSkills { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of user interests and tags.
    /// </summary>
    public ICollection<UserInterest> ProfileTags { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of badges earned by the user.
    /// </summary>
    public ICollection<ProfileBadge> ProfileBadges { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of post votes cast by the user.
    /// </summary>
    public ICollection<PostVote> PostVotes { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of comment votes cast by the user.
    /// </summary>
    public ICollection<CommentVote> CommentVotes { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of job posting votes cast by the user.
    /// </summary>
    public ICollection<JobPostingVote> JobPostingVotes { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of jobs where the user is the author/hirer.
    /// </summary>
    public ICollection<Job> JobsAsAuthor { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of jobs where the user is the assigned worker.
    /// </summary>
    public ICollection<Job> JobsAsWorker { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of job offers created by the user.
    /// </summary>
    public ICollection<JobOffer> AuthoredJobOffers { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of job offers received by the user.
    /// </summary>
    public ICollection<JobOffer> ReceivedJobOffers { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of chats where the user is the worker.
    /// </summary>
    public ICollection<Chat> ChatsAsWorker { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of chats where the user is the hirer.
    /// </summary>
    public ICollection<Chat> ChatsAsHirer { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of messages sent by the user.
    /// </summary>
    public ICollection<Message> SentMessages { get; set; } = [];
}
