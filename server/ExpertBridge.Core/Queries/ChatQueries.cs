// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Chats;

namespace ExpertBridge.Core.Queries;

/// <summary>
/// Provides extension methods for querying Chat entities.
/// </summary>
/// <remarks>
/// These query extensions support filtering chats by participant membership and permissions.
/// </remarks>
public static class ChatQueries
{
    /// <summary>
    /// Filters chats to only those where the specified profile is a participant (either as worker or hirer).
    /// </summary>
    /// <param name="query">The source queryable of chats.</param>
    /// <param name="profileId">The profile ID to check for participation.</param>
    /// <returns>A queryable of chats filtered by participant membership.</returns>
    /// <remarks>
    /// This is essential for security to ensure users can only access chats they are part of.
    /// </remarks>
    public static IQueryable<Chat> WhereProfileIsChatParticipant(this IQueryable<Chat> query, string profileId) =>
        query.Where(c => c.WorkerId == profileId || c.HirerId == profileId);
}
