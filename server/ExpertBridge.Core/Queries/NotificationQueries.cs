// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Notifications;
using ExpertBridge.Core.Responses;

namespace ExpertBridge.Core.Queries;

/// <summary>
/// Provides extension methods for querying and projecting Notification entities.
/// </summary>
/// <remarks>
/// These query extensions project notifications to response DTOs for real-time delivery via SignalR.
/// </remarks>
public static class NotificationQueries
{
    /// <summary>
    /// Projects notifications to NotificationResponse DTOs.
    /// </summary>
    /// <param name="query">The source queryable of notifications.</param>
    /// <returns>A queryable of NotificationResponse objects.</returns>
    public static IQueryable<NotificationResponse> SelectNotificationResopnse(
        this IQueryable<Notification> query) =>
        query
            .Select(n => new NotificationResponse
            {
                Id = n.Id,
                Message = n.Message,
                IsRead = n.IsRead,
                ActionUrl = n.ActionUrl,
                IconUrl = n.IconUrl,
                IconActionUrl = n.IconActionUrl,
                CreatedAt = n.CreatedAt ?? DateTime.UtcNow,
                RecipientId = n.RecipientId
            });
}
