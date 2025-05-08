// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Notifications;
using ExpertBridge.Core.Responses;

namespace ExpertBridge.Core.Queries
{
    public static class NotificationQueries
    {
        public static IQueryable<NotificationResponse> SelectNotificationResopnse(
            this IQueryable<Notification> query)
        {
            return query
                .Select(n => new NotificationResponse
                {
                    Id = n.Id,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    ActionUrl = n.ActionUrl,
                    IconUrl = n.IconUrl,
                    IconActionUrl = n.IconActionUrl,
                    CreatedAt = n.CreatedAt ?? DateTime.UtcNow,
                    RecipientId = n.RecipientId,
                });
        }
    }
}
