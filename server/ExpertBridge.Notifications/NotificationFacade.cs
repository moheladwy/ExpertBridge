// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Messages;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.CommentVotes;
using ExpertBridge.Core.Entities.JobApplications;
using ExpertBridge.Core.Entities.JobOffers;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.JobPostingsVotes;
using ExpertBridge.Core.Entities.Messages;
using ExpertBridge.Core.Entities.ModerationReports;
using ExpertBridge.Core.Entities.Notifications;
using ExpertBridge.Core.Entities.PostVotes;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;
using MassTransit;

namespace ExpertBridge.Notifications;

/// <summary>
///     Provides a facade for creating and sending real-time notifications to users throughout the ExpertBridge platform.
///     Handles notification generation for various platform events (comments, votes, job applications, messages) and
///     queues them for asynchronous delivery via SignalR and database persistence.
/// </summary>
/// <remarks>
///     This facade implements a Channel-based pipeline architecture for notification processing:
///     **Notification Flow:**
///     1. Domain events trigger NotificationFacade methods (e.g., NotifyNewCommentAsync)
///     2. Facade creates Notification entity with recipient, message, and action URLs
///     3. Notification is written to Channel&lt;SendNotificationsRequestMessage&gt;
///     4. NotificationSendingPipelineHandlerWorker reads from channel (background service)
///     5. Worker persists notification to database
///     6. Worker broadcasts notification via SignalR to connected clients
///     **Benefits of Channel-Based Architecture:**
///     - Decouples notification creation from delivery (non-blocking)
///     - Provides backpressure handling for high notification volume
///     - Enables reliable notification processing even during database or SignalR issues
///     - Supports transaction isolation (notifications don't block main transaction)
///     **Notification Types Supported:**
///     - Social interactions: comments, replies, votes on posts/jobs
///     - Job marketplace: applications, offers, job matches
///     - Content moderation: deletion notifications
///     - Real-time chat: new message notifications
///     All notifications include optional icon URLs for sender profile pictures and action URLs for deep linking into the
///     application.
/// </remarks>
public class NotificationFacade
{
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotificationFacade" /> class with the notification channel for
    ///     asynchronous processing.
    /// </summary>
    /// <param name="publishEndpoint"> The MassTransit publish endpoint for inter-service communication.</param>
    public NotificationFacade(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Creates and queues a notification for a new comment on a post or job posting.
    ///     If the comment is a reply to another comment, delegates to <see cref="NotifyNewReplyAsync" /> for multi-recipient
    ///     notification.
    /// </summary>
    /// <param name="comment">The comment entity that was created, including author and post/job posting information.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    /// <remarks>
    ///     For top-level comments, notifies the post/job posting author.
    ///     For reply comments, notifies both the post author and the parent comment author.
    ///     Includes comment content preview, author profile picture, and deep link to the comment.
    /// </remarks>
    public async Task NotifyNewCommentAsync(Comment comment)
    {
        if (comment.ParentComment != null)
        {
            await NotifyNewReplyAsync(comment);
        }
        else
        {
            await NotifyInternalAsync(new Notification
            {
                RecipientId = comment.Post?.AuthorId ?? comment.JobPosting?.AuthorId,
                Message = $"{comment.Author.FirstName} commented on your post: {comment.Content}",
                ActionUrl =
                    $"/{(comment.PostId != null ? "posts" : "jobPostings")}/{comment.PostId ?? comment.JobPostingId}/#comment-{comment.Id}",
                IconUrl = comment.Author.ProfilePictureUrl,
                IconActionUrl = $"/profile/{comment.AuthorId}",
                SenderId = comment.AuthorId
            });
        }
    }

    public async Task NotifyNewReplyAsync(Comment comment)
    {
        var notifications = new List<Notification>
        {
            // Notify post owner
            new()
            {
                RecipientId = comment.Post?.AuthorId ?? comment.JobPosting?.AuthorId,
                Message = $"{comment.Author.FirstName} replied to a comment on your post: {comment.Content}",
                ActionUrl =
                    $"/{(comment.PostId != null ? "posts" : "jobPostings")}/{comment.PostId ?? comment.JobPostingId}/#comment-{comment.Id}",
                IconUrl = comment.Author.ProfilePictureUrl,
                IconActionUrl = $"/profile/{comment.AuthorId}",
                SenderId = comment.AuthorId
            }
        };

        if (comment.ParentComment != null)
        {
            // Notify comment owner (new reply)
            notifications.Add(new Notification
            {
                RecipientId = comment.ParentComment.AuthorId,
                Message = $"{comment.Author.FirstName} replied to your comment: {comment.Content}",
                ActionUrl =
                    $"/{(comment.PostId != null ? "posts" : "jobPostings")}/{comment.PostId ?? comment.JobPostingId}/#comment-{comment.Id}",
                IconUrl = comment.Author.ProfilePictureUrl,
                IconActionUrl = $"/profile/{comment.AuthorId}",
                SenderId = comment.AuthorId
            });
        }

        await NotifyInternalAsync(notifications);
    }

    /// <summary>
    ///     Creates and queues a notification when a comment receives an upvote or downvote.
    ///     Notifies the comment author about the new vote on their comment.
    /// </summary>
    /// <param name="vote">The comment vote entity including the comment and voter profile information.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    /// <remarks>
    ///     Includes a preview of the comment content and a deep link to the comment within its parent post or job posting.
    ///     The notification icon uses the comment author's profile picture.
    /// </remarks>
    public async Task NotifyCommentVotedAsync(CommentVote vote)
    {
        ArgumentNullException.ThrowIfNull(vote);

        await NotifyInternalAsync(new Notification
        {
            RecipientId = vote.Comment.AuthorId,
            Message = $"Your comment \"{vote.Comment.Content}\" recieved a new vote",
            ActionUrl =
                $"{(vote.Comment.PostId != null ? "posts" : "jobPostings")}/{vote.Comment.PostId ?? vote.Comment.JobPostingId}/#comment-{vote.Comment.Id}",
            IconUrl = vote.Comment.Author.ProfilePictureUrl,
            IconActionUrl = "/profile",
            SenderId = vote.ProfileId
        });
    }

    /// <summary>
    ///     Creates and queues a notification when a post receives an upvote or downvote.
    ///     Notifies the post author about the new vote on their content.
    /// </summary>
    /// <param name="vote">The post vote entity including the post and voter profile information.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    /// <remarks>
    ///     Includes the post title and a deep link to the post detail page.
    ///     The notification icon uses the post author's profile picture.
    /// </remarks>
    public async Task NotifyPostVotedAsync(PostVote vote)
    {
        ArgumentNullException.ThrowIfNull(vote);

        await NotifyInternalAsync(new Notification
        {
            RecipientId = vote.Post.AuthorId,
            Message = $"Your post \"{vote.Post.Title}\" recieved a new vote",
            ActionUrl = $"/posts/{vote.Post.Id}",
            IconUrl = vote.Post.Author.ProfilePictureUrl,
            IconActionUrl = "/profile",
            SenderId = vote.ProfileId
        });
    }

    /// <summary>
    ///     Creates and queues a notification when a job posting receives an upvote or downvote.
    ///     Notifies the job posting author about the new vote on their job listing.
    /// </summary>
    /// <param name="vote">The job posting vote entity including the job posting and voter profile information.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    /// <remarks>
    ///     Includes the job posting title and a deep link to the job detail page.
    ///     The notification icon uses the job posting author's profile picture.
    /// </remarks>
    public async Task NotifyJobPostingVotedAsync(JobPostingVote vote)
    {
        ArgumentNullException.ThrowIfNull(vote);

        await NotifyInternalAsync(new Notification
        {
            RecipientId = vote.JobPosting.AuthorId,
            Message = $"Your job \"{vote.JobPosting.Title}\" recieved a new vote",
            ActionUrl = $"/job/{vote.JobPosting.Id}",
            IconUrl = vote.JobPosting.Author.ProfilePictureUrl,
            IconActionUrl = "/profile",
            SenderId = vote.ProfileId
        });
    }


    /// <summary>
    ///     Creates and queues a notification when a comment is deleted by content moderation.
    ///     Informs the comment author about the removal and the reason from the moderation report.
    /// </summary>
    /// <param name="comment">The deleted comment entity including author and content information.</param>
    /// <param name="report">The moderation report entity containing the deletion reason.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    /// <remarks>
    ///     Includes the moderation reason and a preview of the deleted comment content.
    ///     Links to the user's profile page. Part of the AI-assisted content moderation system using Groq API.
    /// </remarks>
    public async Task NotifyCommentDeletedAsync(Comment comment, ModerationReport report)
    {
        await NotifyInternalAsync(new Notification
        {
            RecipientId = comment.AuthorId,
            Message = $"Your comment was removed: {report.Reason}.\nComment: {comment.Content}",
            IconUrl = comment.Post?.Author.ProfilePictureUrl ?? comment.JobPosting?.Author.ProfilePictureUrl,
            ActionUrl = "/profile"
        });
    }

    /// <summary>
    ///     Sends a notification to the author of a comment when their comment has been restored
    ///     following a review by the site administrators.
    /// </summary>
    /// <param name="comment">
    ///     The comment that has been restored, containing details such as its content and related post or
    ///     job posting.
    /// </param>
    /// <param name="report">The moderation report associated with the review and restoration of the comment.</param>
    /// <returns>A task representing the asynchronous operation of sending the notification.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if either the <paramref name="comment" /> or <paramref name="report" /> is null.
    /// </exception>
    public async Task NotifyCommentRestoredAsync(Comment comment, ModerationReport report)
    {
        ArgumentNullException.ThrowIfNull(comment);
        ArgumentNullException.ThrowIfNull(report);
        await NotifyInternalAsync(new Notification
        {
            RecipientId = comment.AuthorId,
            Message =
                $"Your comment has been restored after being reviewed by the site admins.\nComment: {comment.Content}",
            IconUrl = comment.Post?.Author.ProfilePictureUrl ?? comment.JobPosting?.Author.ProfilePictureUrl,
            ActionUrl = $"/posts/{comment.PostId ?? comment.JobPostingId}/#comment-{comment.Id}"
        });
    }

    /// <summary>
    ///     Creates and queues a notification when a post or job posting is deleted by content moderation.
    ///     Informs the author about the removal and the reason from the moderation report.
    /// </summary>
    /// <param name="post">The deleted post or job posting entity implementing IRecommendableContent interface.</param>
    /// <param name="report">The moderation report entity containing the deletion reason.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    /// <remarks>
    ///     Includes the moderation reason and the post/job title.
    ///     Links to the user's profile page. Part of the AI-assisted content moderation system using Groq API.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if either the <paramref name="post" /> or <paramref name="report" /> is null.
    /// </exception>
    public async Task NotifyPostDeletedAsync(IRecommendableContent post, ModerationReport report)
    {
        ArgumentNullException.ThrowIfNull(post);
        ArgumentNullException.ThrowIfNull(report);
        await NotifyInternalAsync(new Notification
        {
            RecipientId = post.AuthorId,
            Message = $"Your post was removed: {report.Reason}.\nPost: {post.Title}",
            ActionUrl = "/profile"
        });
    }

    /// <summary>
    ///     Sends a notification to the author of a moderated post, informing them that their post has
    ///     been restored after a review by the site administrators.
    /// </summary>
    /// <param name="post">The content that has been restored, implementing the <see cref="IRecommendableContent" /> interface.</param>
    /// <param name="report">
    ///     The moderation report corresponding to the restored content, containing author details and related
    ///     metadata.
    /// </param>
    /// <returns>A task that represents the asynchronous operation of sending the notification.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if either the <paramref name="post" /> or <paramref name="report" /> is null.
    /// </exception>
    public async Task NotifyPostRestoredAsync(IRecommendableContent post, ModerationReport report)
    {
        ArgumentNullException.ThrowIfNull(post);
        ArgumentNullException.ThrowIfNull(report);
        var contentType = report.ContentType == ContentTypes.Post ? "post" : "job post";

        await NotifyInternalAsync(new Notification
        {
            RecipientId = report.AuthorId,
            Message =
                $"Your {contentType} has been restored after being reviewed by the site admins, Title: {post.Title}",
            ActionUrl = $"/{(report.ContentType == ContentTypes.Post ? "posts" : "jobs")}/{post.Id}"
        });
    }

    /// <summary>
    ///     Creates and queues bulk notifications to multiple candidates when a new job posting matches their profiles.
    ///     Notifies all matching candidates about a new job opportunity that aligns with their skills and interests.
    /// </summary>
    /// <param name="jobPosting">The job posting entity including title and requirements information.</param>
    /// <param name="candidates">
    ///     The list of expert profiles that match the job posting criteria based on AI similarity
    ///     matching.
    /// </param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    /// <remarks>
    ///     This batch notification method:
    ///     - Filters out the job posting author from the candidate list (no self-notification)
    ///     - Creates a notification for each matching candidate
    ///     - Includes the job title and a deep link to the job detail page
    ///     - Uses AI-powered profile/job matching via vector embeddings (Pgvector cosine similarity)
    ///     Typical usage:
    ///     When a client posts a new job, the system runs background processing to find experts with matching
    ///     skills/interests,
    ///     then notifies all qualified candidates about the opportunity for increased application rate.
    ///     Performance note: Efficient batching allows notifying many candidates in a single operation.
    /// </remarks>
    public async Task NotifyJobMatchAsync(JobPosting jobPosting, List<Profile> candidates)
    {
        var notifications = candidates
            .Where(p => p.Id != jobPosting.AuthorId)
            .Select(candidate => new Notification
            {
                RecipientId = candidate.Id,
                Message = $"Check this new job which matches your profile: {jobPosting.Title}",
                ActionUrl = $"/job/{jobPosting.Id}"
            })
            .ToList();

        await NotifyInternalAsync(notifications);
    }

    /// <summary>
    ///     Creates and queues a notification when an expert submits an application to a job posting.
    ///     Notifies the job posting author about the new application from a candidate.
    /// </summary>
    /// <param name="jobApplication">The job application entity including applicant profile and job posting information.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    /// <remarks>
    ///     Includes the applicant's name, job title, and a deep link to the job applications page.
    ///     The notification icon uses the applicant's profile picture.
    /// </remarks>
    public async Task NotifyJobApplicationSubmittedAsync(JobApplication jobApplication)
    {
        await NotifyInternalAsync(new Notification
        {
            RecipientId = jobApplication.JobPosting.AuthorId,
            Message =
                $"{jobApplication.Applicant.FirstName} applied for your job: {jobApplication.JobPosting.Title}",
            ActionUrl = $"/job/{jobApplication.JobPosting.Id}/applications",
            IconUrl = jobApplication.Applicant.ProfilePictureUrl,
            IconActionUrl = $"/profile/{jobApplication.ApplicantId}",
            SenderId = jobApplication.ApplicantId
        });
    }

    /// <summary>
    ///     Creates and queues a notification when a client creates a job offer for an expert.
    ///     Notifies the selected expert that they have received a direct job offer.
    /// </summary>
    /// <param name="offer">The job offer entity including author (client) and worker (expert) profile information.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    /// <remarks>
    ///     Includes the client's name, offer title, and a deep link to the offers page.
    ///     The notification icon uses the client's profile picture.
    /// </remarks>
    public async Task NotifyJobOfferCreatedAsync(JobOffer offer)
    {
        await NotifyInternalAsync(new Notification
        {
            RecipientId = offer.WorkerId,
            Message = $"{offer.Author.FirstName} wants to hire you: {offer.Title}",
            ActionUrl = "/offers",
            IconUrl = offer.Author.ProfilePictureUrl,
            IconActionUrl = $"/profile/{offer.AuthorId}",
            SenderId = offer.AuthorId
        });
    }

    /// <summary>
    ///     Creates and queues a notification when a user receives a new chat message.
    ///     Notifies the recipient about the new message with a preview of the content.
    /// </summary>
    /// <param name="message">The message entity including sender profile and message content.</param>
    /// <param name="receiverId">The profile ID of the message recipient.</param>
    /// <param name="jobId">The job ID associated with the chat conversation for deep linking.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    /// <remarks>
    ///     Includes the sender's name, message preview, and a deep link to the chat within the job context.
    ///     The notification icon uses the sender's profile picture.
    /// </remarks>
    public async Task NotifyNewMessageReceivedAsync(Message message, string receiverId, string jobId)
    {
        await NotifyInternalAsync(new Notification
        {
            RecipientId = receiverId,
            Message = $"{message.Sender.FirstName} sent you a message: {message.Content}",
            ActionUrl = $"/my-jobs/{jobId}",
            IconUrl = message.Sender.ProfilePictureUrl,
            IconActionUrl = $"/profile/{message.SenderId}",
            SenderId = message.SenderId
        });
    }

    /// <summary>
    ///     Internal method that filters notifications to prevent self-notification and writes them to the Channel for
    ///     asynchronous processing.
    ///     Transforms Core.Entities.Notifications.Notification to SendNotificationMessage for channel communication.
    /// </summary>
    /// <param name="notifications">Variable number of notification lists to be queued for processing.</param>
    /// <returns>A task representing the asynchronous channel write operation.</returns>
    /// <remarks>
    ///     This method:
    ///     1. Filters out notifications where recipient equals sender (prevent self-notification)
    ///     2. Transforms notifications into SendNotificationMessage DTOs for IPC
    ///     3. Writes to unbounded channel for consumption by NotificationSendingPipelineHandlerWorker
    ///     4. Returns immediately without blocking (fire-and-forget pattern)
    ///     The commented-out database persistence code has been moved to the background worker for better separation of
    ///     concerns.
    /// </remarks>
    private async Task NotifyInternalAsync(params List<Notification> notifications)
    {
        var toSend = notifications
            .Where(n => n.RecipientId != n.SenderId);
        ;

        await _publishEndpoint.Publish(new SendNotificationsRequestMessage
        {
            Notifications = toSend.Select(notification => new SendNotificationMessage
            {
                RecipientId = notification.RecipientId,
                SenderId = notification.SenderId,
                Message = notification.Message,
                ActionUrl = notification.ActionUrl,
                IconUrl = notification.IconUrl,
                IconActionUrl = notification.IconActionUrl,
                IsRead = notification.IsRead
            }).ToList()
        });
    }
}
