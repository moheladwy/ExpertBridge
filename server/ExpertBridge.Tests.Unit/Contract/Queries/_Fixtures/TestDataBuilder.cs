// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Bogus;
using ExpertBridge.Core.Entities.Chats;
using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.CommentVotes;
using ExpertBridge.Core.Entities.JobApplications;
using ExpertBridge.Core.Entities.JobOffers;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.JobStatuses;
using ExpertBridge.Core.Entities.Media;
using ExpertBridge.Core.Entities.Media.PostMedia;
using ExpertBridge.Core.Entities.Messages;
using ExpertBridge.Core.Entities.Notifications;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Tags;

namespace ExpertBridge.Tests.Unit.Contract.Queries._Fixtures;

/// <summary>
/// Builder for creating realistic test data using Bogus library.
/// </summary>
public static class TestDataBuilder
{
    private static readonly Faker _faker = new();

    /// <summary>
    /// Creates a test tag with English and optional Arabic name.
    /// </summary>
    public static Tag CreateTag(string englishName, string? arabicName = null, string? id = null)
    {
        return new Tag
        {
            Id = id ?? Guid.NewGuid().ToString(),
            EnglishName = englishName,
            ArabicName = arabicName,
            Description = _faker.Lorem.Sentence()
        };
    }

    /// <summary>
    /// Creates a test PostMedia or CommentMedia object with type and optional URL or key.
    /// </summary>
#pragma warning disable CA1054 // URI parameters should not be strings (test data helper)
    public static MediaObject CreatePostMedia(
        string? postId = null,
        string? commentId = null,
        string? jobPostingId = null,
        string? type = null,
        string? key = null,
        string? url = null,
        string? id = null)
#pragma warning restore CA1054
    {
        // If URL is provided, extract the key from it (format: https://expert-bridge-media.s3.amazonaws.com/{key})
        if (url != null && key == null)
        {
            var parts = url.Split('/');
            key = parts[^1]; // Last part is the key
        }

        // If key is not provided, generate a random one
        key ??= _faker.System.FileName();

        // Use provided type or default to image/jpeg
        string mediaTypeString = type ?? "image/jpeg";

        if (commentId != null)
        {
            return new ExpertBridge.Core.Entities.Media.CommentMedia.CommentMedia
            {
                Id = id ?? Guid.NewGuid().ToString(),
                Name = _faker.System.FileName(),
                Type = mediaTypeString,
                Key = key,
                CommentId = commentId
            };
        }

        if (postId != null)
        {
            return new PostMedia
            {
                Id = id ?? Guid.NewGuid().ToString(),
                Name = _faker.System.FileName(),
                Type = mediaTypeString,
                Key = key,
                PostId = postId
            };
        }

        if (jobPostingId != null)
        {
            return new ExpertBridge.Core.Entities.Media.JobPostingMedia.JobPostingMedia
            {
                Id = id ?? Guid.NewGuid().ToString(),
                Name = _faker.System.FileName(),
                Type = mediaTypeString,
                Key = key,
                JobPostingId = jobPostingId
            };
        }

        // Default to PostMedia with a dummy postId
        return new PostMedia
        {
            Id = id ?? Guid.NewGuid().ToString(),
            Name = _faker.System.FileName(),
            Type = mediaTypeString,
            Key = key,
            PostId = "dummy-post-id"
        };
    }    /// <summary>
         /// Creates a test chat with required hirer and worker IDs.
         /// </summary>
    public static Chat CreateChat(string hirerId, string workerId, string? id = null)
    {
        return new Chat
        {
            Id = id ?? Guid.NewGuid().ToString(),
            HirerId = hirerId,
            WorkerId = workerId,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a test message with required sender, chat, and content.
    /// </summary>
    public static Message CreateMessage(string senderId, string chatId, string? content = null, bool isConfirmation = false, string? id = null)
    {
        return new Message
        {
            Id = id ?? Guid.NewGuid().ToString(),
            SenderId = senderId,
            ChatId = chatId,
            Content = content ?? _faker.Lorem.Sentence(),
            IsConfirmationMessage = isConfirmation,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a test notification.
    /// </summary>
#pragma warning disable CA1054 // URI parameters should not be strings (entity uses string type)
    public static Notification CreateNotification(
        string recipientId,
        string? message = null,
        bool isRead = false,
        string? actionUrl = null,
        string? iconUrl = null,
        string? iconActionUrl = null,
        string? senderId = null,
        string? id = null)
#pragma warning restore CA1054
    {
        return new Notification
        {
            Id = id ?? Guid.NewGuid().ToString(),
            RecipientId = recipientId,
            SenderId = senderId,
            Message = message ?? _faker.Lorem.Sentence(),
            IsRead = isRead,
            ActionUrl = actionUrl,
            IconUrl = iconUrl,
            IconActionUrl = iconActionUrl,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a test profile with required fields.
    /// </summary>
#pragma warning disable CA1054 // URI parameters should not be strings (entity uses string type)
    public static Profile CreateProfile(
        string userId,
        string? email = null,
        string? username = null,
        string? firstName = null,
        string? lastName = null,
        string? jobTitle = null,
        string? profilePictureUrl = null,
        string? id = null)
#pragma warning restore CA1054
    {
        return new Profile
        {
            Id = id ?? Guid.NewGuid().ToString(),
            UserId = userId,
            Email = email ?? _faker.Internet.Email(),
            Username = username ?? _faker.Internet.UserName(),
            FirstName = firstName ?? _faker.Name.FirstName(),
            LastName = lastName ?? _faker.Name.LastName(),
            JobTitle = jobTitle,
            ProfilePictureUrl = profilePictureUrl,
            Rating = _faker.Random.Double(0, 5),
            RatingCount = _faker.Random.Int(0, 100),
            Comments = []
        };
    }

    /// <summary>
    /// Creates a test comment with required fields.
    /// </summary>
    public static Comment CreateComment(
        string authorId,
        string? content = null,
        string? postId = null,
        string? jobPostingId = null,
        string? parentCommentId = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null,
        string? id = null)
    {
        return new Comment
        {
            Id = id ?? Guid.NewGuid().ToString(),
            AuthorId = authorId,
            Content = content ?? _faker.Lorem.Sentence(),
            PostId = postId,
            JobPostingId = jobPostingId,
            ParentCommentId = parentCommentId,
            IsProcessed = true,
            CreatedAt = createdAt ?? DateTime.UtcNow,
            UpdatedAt = updatedAt,
            Votes = [],
            Replies = [],
            Medias = []
        };
    }

    /// <summary>
    /// Creates a test comment vote.
    /// </summary>
    public static CommentVote CreateCommentVote(
        string commentId,
        string profileId,
        bool isUpvote = true,
        string? id = null)
    {
        return new CommentVote
        {
            Id = id ?? Guid.NewGuid().ToString(),
            CommentId = commentId,
            ProfileId = profileId,
            IsUpvote = isUpvote
        };
    }

    /// <summary>
    /// Creates a test job posting with required fields.
    /// </summary>
    public static JobPosting CreateJobPosting(
        string authorId,
        string title,
        string content,
        decimal budget,
        string? area = null,
        string? id = null)
    {
        return new JobPosting
        {
            Id = id ?? Guid.NewGuid().ToString(),
            AuthorId = authorId,
            Title = title,
            Content = content,
            Budget = budget,
            Area = area ?? _faker.Address.City(),
            IsProcessed = true,
            IsSafeContent = true
        };
    }

    /// <summary>
    /// Creates a test job application with required fields.
    /// </summary>
    public static JobApplication CreateJobApplication(
        string jobPostingId,
        string applicantId,
        decimal offeredCost,
        string? coverLetter = "Default Cover Letter",
        string? id = null)
    {
        return new JobApplication
        {
            Id = id ?? Guid.NewGuid().ToString(),
            JobPostingId = jobPostingId,
            ApplicantId = applicantId,
            OfferedCost = offeredCost,
            CoverLetter = coverLetter == "Default Cover Letter" ? _faker.Lorem.Paragraph() : coverLetter,
            IsDeleted = false
        };
    }

    /// <summary>
    /// Creates a test job offer with required fields.
    /// </summary>
    public static JobOffer CreateJobOffer(
        string authorId,
        string workerId,
        string title,
        string description,
        decimal budget,
        string area,
        bool isAccepted = false,
        bool isDeclined = false,
        string? id = null)
    {
        return new JobOffer
        {
            Id = id ?? Guid.NewGuid().ToString(),
            AuthorId = authorId,
            WorkerId = workerId,
            Title = title,
            Description = description,
            Budget = budget,
            Area = area,
            IsAccepted = isAccepted,
            IsDeclined = isDeclined,
            IsDeleted = false
        };
    }

    public static Job CreateJob(
        string authorId,
        string workerId,
        string? title = null,
        string? description = null,
        string? area = null,
        string? chatId = null,
        decimal actualCost = 0m,
        DateTime? startedAt = null,
        DateTime? endedAt = null,
        DateTime? updatedAt = null,
        bool isPaid = false,
        bool isCompleted = false,
        JobStatusEnum status = JobStatusEnum.Offered,
        string? id = null)
    {
        return new Job
        {
            Id = id ?? Guid.NewGuid().ToString(),
            AuthorId = authorId,
            WorkerId = workerId,
            Title = title ?? "Test Job Title",
            Description = description ?? "Test Job Description",
            Area = area ?? "Test Area",
            ChatId = chatId ?? Guid.NewGuid().ToString(),
            ActualCost = actualCost,
            StartedAt = startedAt,
            EndedAt = endedAt,
            UpdatedAt = updatedAt,
            IsPaid = isPaid,
            IsCompleted = isCompleted,
            Status = status,
            IsDeleted = false
        };
    }

    /// <summary>
    /// Creates a test post with required fields.
    /// </summary>
    public static ExpertBridge.Core.Entities.Posts.Post CreatePost(
        string authorId,
        string title,
        string content,
        string? language = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null,
        bool isProcessed = true,
        bool isSafeContent = true,
        bool isDeleted = false,
        string? id = null)
    {
        return new ExpertBridge.Core.Entities.Posts.Post
        {
            Id = id ?? Guid.NewGuid().ToString(),
            AuthorId = authorId,
            Title = title,
            Content = content,
            Language = language ?? "en",
            IsProcessed = isProcessed,
            IsSafeContent = isSafeContent,
            IsDeleted = isDeleted,
            CreatedAt = createdAt ?? DateTime.UtcNow,
            UpdatedAt = updatedAt,
            Votes = [],
            Comments = [],
            Medias = [],
            PostTags = []
        };
    }

    /// <summary>
    /// Creates a test post vote.
    /// </summary>
    public static ExpertBridge.Core.Entities.PostVotes.PostVote CreatePostVote(
        string postId,
        string profileId,
        bool isUpvote = true,
        string? id = null)
    {
        return new ExpertBridge.Core.Entities.PostVotes.PostVote
        {
            Id = id ?? Guid.NewGuid().ToString(),
            PostId = postId,
            ProfileId = profileId,
            IsUpvote = isUpvote
        };
    }

    /// <summary>
    /// Creates a test post tag relationship.
    /// </summary>
    public static ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags.PostTag CreatePostTag(
        string postId,
        string tagId)
    {
        return new ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags.PostTag
        {
            PostId = postId,
            TagId = tagId
        };
    }

    /// <summary>
    /// Creates a test job posting vote.
    /// </summary>
    public static ExpertBridge.Core.Entities.JobPostingsVotes.JobPostingVote CreateJobPostingVote(
        string jobPostingId,
        string profileId,
        bool isUpvote = true,
        string? id = null)
    {
        return new ExpertBridge.Core.Entities.JobPostingsVotes.JobPostingVote
        {
            Id = id ?? Guid.NewGuid().ToString(),
            JobPostingId = jobPostingId,
            ProfileId = profileId,
            IsUpvote = isUpvote
        };
    }

    /// <summary>
    /// Creates a test job posting tag relationship.
    /// </summary>
    public static ExpertBridge.Core.Entities.ManyToManyRelationships.JobPostingTags.JobPostingTag CreateJobPostingTag(
        string jobPostingId,
        string tagId)
    {
        return new ExpertBridge.Core.Entities.ManyToManyRelationships.JobPostingTags.JobPostingTag
        {
            JobPostingId = jobPostingId,
            TagId = tagId
        };
    }

    /// <summary>
    /// Creates a test job posting media.
    /// </summary>
    public static ExpertBridge.Core.Entities.Media.JobPostingMedia.JobPostingMedia CreateJobPostingMedia(
        string jobPostingId,
        string? key = null,
        string? type = null,
        string? name = null,
        string? id = null)
    {
        return new ExpertBridge.Core.Entities.Media.JobPostingMedia.JobPostingMedia
        {
            Id = id ?? Guid.NewGuid().ToString(),
            JobPostingId = jobPostingId,
            Key = key ?? "test-media-key",
            Type = type ?? "image/jpeg",
            Name = name ?? "test-image.jpg",
            IsDeleted = false
        };
    }
}
