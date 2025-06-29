// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.CommentVotes;
using ExpertBridge.Core.Entities.ModerationReports;
using ExpertBridge.Core.Entities.Notifications;
using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Entities.PostVotes;
using ExpertBridge.Notifications.Models.IPC;

namespace ExpertBridge.Notifications
{
    /// <summary>
    /// This Facade is responsible for handling notifiactions.
    /// Sending them to the notification channel for processing by the Hub <br/>
    /// and sending them to the database for persistence.
    /// </summary>
    public class NotificationFacade
    {
        private readonly Channel<SendNotificationsRequestMessage> _channel;

        public NotificationFacade(
            Channel<SendNotificationsRequestMessage> channel)
        {
            _channel = channel;
        }

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
                    RecipientId = comment.Post.AuthorId,
                    Message = $"{comment.Author.FirstName} commented on your post: {comment.Content}",
                    ActionUrl = $"/posts/{comment.PostId}/#comment-{comment.Id}",
                    IconUrl = comment.Author.ProfilePictureUrl,
                    IconActionUrl = $"/profiles/{comment.AuthorId}",
                    SenderId = comment.AuthorId,
                });
            }
        }

        public async Task NotifyNewReplyAsync(Comment comment)
        {
            var notifications = new List<Notification>
            {
                // Notify post owner
                new Notification
                {
                    RecipientId = comment.Post.AuthorId,
                    Message = $"{comment.Author.FirstName} replied to a comment on your post: {comment.Content}",
                    ActionUrl = $"/posts/{comment.PostId}/#comment-{comment.Id}",
                    IconUrl = comment.Author.ProfilePictureUrl,
                    IconActionUrl = $"/profiles/{comment.AuthorId}",
                    SenderId = comment.AuthorId,
                }
            };

            if (comment.ParentComment != null)
            {
                // Notify comment owner (new reply)
                notifications.Add(new Notification
                {
                    RecipientId = comment.ParentComment.AuthorId,
                    Message = $"{comment.Author.FirstName} replied to you comment: {comment.Content}",
                    ActionUrl = $"/posts/{comment.PostId}/#comment-{comment.Id}",
                    IconUrl = comment.Author.ProfilePictureUrl,
                    IconActionUrl = $"/profiles/{comment.AuthorId}",
                    SenderId = comment.AuthorId,
                });
            }

            await NotifyInternalAsync(notifications);
        }

        public async Task NotifyCommentVotedAsync(CommentVote vote)
        {
            ArgumentNullException.ThrowIfNull(vote);

            await NotifyInternalAsync(new Notification
            {
                RecipientId = vote.Comment.AuthorId,
                Message = $"Your comment \"{vote.Comment.Content}\" recieved a new vote",
                ActionUrl = $"/posts/{vote.Comment.PostId}/#comment-{vote.Comment.Id}",
                IconUrl = vote.Comment.Author.ProfilePictureUrl,
                IconActionUrl = $"/profile",
                SenderId = vote.ProfileId,
            });
        }

        public async Task NotifyPostVotedAsync(PostVote vote)
        {
            ArgumentNullException.ThrowIfNull(vote);

            await NotifyInternalAsync(new Notification
            {
                RecipientId = vote.Post.AuthorId,
                Message = $"Your post \"{vote.Post.Title}\" recieved a new vote",
                ActionUrl = $"/posts/{vote.Post.Id}",
                IconUrl = vote.Post.Author.ProfilePictureUrl,
                IconActionUrl = $"/profile",
                SenderId = vote.ProfileId,
            });
        }

        public async Task NotifyCommentDeletedAsync(Comment comment, ModerationReport report)
        {
            await NotifyInternalAsync(new Notification
            {
                RecipientId = comment.AuthorId,
                Message = $"Your comment was removed: {report.Reason}.\nComment: {comment.Content}",
                ActionUrl = $"/profile",
            });
        }

        public async Task NotifyPostDeletedAsync(IRecommendableContent post, ModerationReport report)
        {
            await NotifyInternalAsync(new Notification
            {
                RecipientId = post.AuthorId,
                Message = $"Your post was removed: {report.Reason}.\nPost: {post.Title}",
                ActionUrl = $"/profile",
            });
        }

        private async Task NotifyInternalAsync(params List<Notification> notifications)
        {
            var toSend = notifications
                .Where(n => n.RecipientId != n.SenderId);

            //await _dbContext.Notifications.AddRangeAsync(toSend);
            //await _dbContext.SaveChangesAsync();

            await _channel.Writer.WriteAsync(new SendNotificationsRequestMessage
            {
                Notifications = toSend.Select(notification => new SendNotificationMessage
                {
                    RecipientId = notification.RecipientId,
                    SenderId = notification.SenderId,
                    Message = notification.Message,
                    ActionUrl = notification.ActionUrl,
                    IconUrl = notification.IconUrl,
                    IconActionUrl = notification.IconActionUrl,
                    IsRead = notification.IsRead,
                }).ToList()
            });
        }
    }
}
