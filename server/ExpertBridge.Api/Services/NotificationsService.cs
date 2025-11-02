// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Queries;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Services;

/// <summary>
///     Service for managing user notifications.
/// </summary>
public sealed class NotificationsService
{
    /// <summary>
    ///     The database context instance used to access and manage the application's database entities.
    ///     Provides an abstraction for querying and saving data to the database, as well as handling
    ///     database-related operations. This instance is injected into the service for dependency management.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotificationsService" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing notification data.</param>
    public NotificationsService(ExpertBridgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    ///     Retrieves all notifications for the specified user.
    /// </summary>
    /// <param name="currentUser">The user whose notifications should be retrieved.</param>
    /// <returns>A list of notification responses for the user.</returns>
    /// <exception cref="ArgumentNullException">Thrown when currentUser or currentUser.Profile is null.</exception>
    public async Task<List<NotificationResponse>> GetAll(User currentUser)
    {
        return await _dbContext.Notifications
            .Where(n => n.RecipientId == currentUser.Profile.Id)
            .SelectNotificationResopnse()
            .ToListAsync();
    }

    /// <summary>
    ///     Marks all unread notifications as read for the specified user.
    /// </summary>
    /// <param name="currentUser">The user whose notifications should be marked as read.</param>
    /// <exception cref="ArgumentNullException">Thrown when currentUser or currentUser.Profile is null.</exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">
    ///     Thrown when an error is encountered while saving to
    ///     the database.
    /// </exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">
    ///     Thrown when a concurrency violation is
    ///     encountered while updating the database.
    /// </exception>
    public async Task MarkAllRead(User currentUser)
    {
        await _dbContext.Notifications
            .Where(n => n.RecipientId == currentUser.Profile.Id && !n.IsRead)
            .ExecuteUpdateAsync(set => set.SetProperty(n => n.IsRead, true));
    }
}
