// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using Core.Entities.Comments;
using Core.Entities.CommentVotes;
using Core.Entities.Notifications;
using Core.Entities.Posts;
using Core.Entities.PostVotes;
using Data.DatabaseContexts;
using Notifications.Models.IPC;

namespace Api.Services
{
    /// <summary>
    /// This Facade is responsible for handling notifiactions.
    /// <br/>
    /// 1. Storing them in the database.
    /// <br/>
    /// 2. Sending them to the notification channel for processing by the Hub.
    /// </summary>
    public class NotificationFacade
    {
        private readonly ExpertBridgeDbContext _dbContext;
        private readonly Channel<SendNotificationMessage> _channel;

        public NotificationFacade(
            ExpertBridgeDbContext dbContext,
            Channel<SendNotificationMessage> channel)
        {
            _dbContext = dbContext;
            _channel = channel;
        }

        public async Task NotifyNewCommentAsync(Comment comment)
        {
            await NotifyInternalAsync(new Notification
            {
                RecipientId = comment.Post.AuthorId,
                Message = $"{comment.Author.FirstName} commented on your post: {comment.Content}",
                ActionUrl = $"/posts/{comment.PostId}",
                IconUrl = comment.Author.ProfilePictureUrl,
                IconActionUrl = $"/profiles/{comment.AuthorId}",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString(),
            });
        }

        public async Task NotifyNewReplyAsync(Comment comment)
        {

        }

        public async Task NotifyCommentVotedAsync(CommentVote vote)
        {

        }

        public async Task NotifyPostVotedAsync(PostVote vote)
        {

        }

        public async Task NotifyCommentDeletedAsync(Comment comment)
        {

        }

        public async Task NotifyPostDeletedAsync(Post post)
        {

        }

        private async Task NotifyInternalAsync(params List<Notification> notifications)
        {
            await _dbContext.Notifications.AddRangeAsync(notifications);
            await _dbContext.SaveChangesAsync();

            foreach (var notification in notifications)
            {
                await _channel.Writer.WriteAsync(new SendNotificationMessage
                {
                    NotificationId = notification.Id,
                    RecipientId = notification.RecipientId,
                    Message = notification.Message,
                    ActionUrl = notification.ActionUrl,
                    IconUrl = notification.IconUrl,
                    IconActionUrl = notification.IconActionUrl,
                    IsRead = notification.IsRead,
                    CreatedAt = notification.CreatedAt ?? DateTime.UtcNow,
                });
            }
        }
    }
}
