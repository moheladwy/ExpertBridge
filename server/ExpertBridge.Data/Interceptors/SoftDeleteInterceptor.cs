// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ExpertBridge.Data.Interceptors;

/// <summary>
/// Entity Framework Core interceptor that implements the soft delete pattern for entities implementing <see cref="ISoftDeletable"/> interface.
/// Intercepts delete operations and converts them to update operations that set IsDeleted flag and DeletedAt timestamp instead of physically removing records from the database.
/// </summary>
/// <remarks>
/// This interceptor provides the following benefits:
/// - Data preservation: Deleted records remain in the database for audit trails, recovery, and compliance
/// - Query filtering: Works with global query filters in ExpertBridgeDbContext to automatically exclude soft-deleted entities
/// - Referential integrity: Maintains foreign key relationships without cascade delete complications
/// - Temporal tracking: Records exact deletion timestamp for analytics and data lifecycle management
///
/// The interceptor is registered in the database configuration via the AddDatabase extension method in Extensions.cs.
/// All entities implementing ISoftDeletable (User, Profile, Post, Comment, JobPosting, etc.) automatically benefit from this pattern.
/// </remarks>
public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Intercepts synchronous SaveChanges operations to convert delete operations into soft deletes for entities implementing <see cref="ISoftDeletable"/>.
    /// Changes entity state from Deleted to Modified and sets IsDeleted flag and DeletedAt timestamp.
    /// </summary>
    /// <param name="eventData">The event data containing the DbContext and tracked entity changes.</param>
    /// <param name="result">The current interception result that can be modified or returned as-is.</param>
    /// <returns>The interception result indicating whether to continue with the save operation.</returns>
    /// <remarks>
    /// This method processes all tracked entities marked for deletion, converts them to modified state, and applies soft delete properties.
    /// Non-soft-deletable entities are not affected and continue with normal hard delete behavior.
    /// </remarks>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData?.Context is null)
        {
            return result;
        }

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry is not { State: EntityState.Deleted, Entity: ISoftDeletable entity })
            {
                continue;
            }

            entry.State = EntityState.Modified;
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }

        return result;
    }

    /// <summary>
    /// Intercepts asynchronous SaveChangesAsync operations to convert delete operations into soft deletes for entities implementing <see cref="ISoftDeletable"/>.
    /// Changes entity state from Deleted to Modified and sets IsDeleted flag and DeletedAt timestamp using UTC time.
    /// </summary>
    /// <param name="eventData">The event data containing the DbContext and tracked entity changes.</param>
    /// <param name="result">The current interception result that can be modified or returned as-is.</param>
    /// <param name="cancellationToken">The cancellation token to observe for operation cancellation.</param>
    /// <returns>A task representing the asynchronous operation, containing the interception result.</returns>
    /// <remarks>
    /// This is the async version of the soft delete interceptor, typically used in web applications and async data access patterns.
    /// It processes all tracked entities marked for deletion, converts them to modified state, and applies soft delete properties.
    /// Non-soft-deletable entities are not affected and continue with normal hard delete behavior.
    /// </remarks>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData?.Context is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry is not { State: EntityState.Deleted, Entity: ISoftDeletable entity })
            {
                continue;
            }

            entry.State = EntityState.Modified;
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }

        return result;
    }
}
