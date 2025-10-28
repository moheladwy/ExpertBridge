// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Entities.Messages;

namespace ExpertBridge.Contract.Queries;

/// <summary>
///     Provides extension methods for querying and projecting Message entities.
/// </summary>
/// <remarks>
///     These query extensions project chat messages to response DTOs for real-time messaging via SignalR.
/// </remarks>
public static class MessageQueries
{
    /// <summary>
    ///     Projects a queryable of Message entities to MessageResponse DTOs for SignalR delivery.
    /// </summary>
    /// <param name="query">The source queryable of messages.</param>
    /// <returns>A queryable of MessageResponse objects with message content and metadata.</returns>
    public static IQueryable<MessageResponse> SelectMessageResponseFromFullMessage(this IQueryable<Message> query)
    {
        return query
            .Select(m => SelectMessageResponseFromFullMessage(m));
    }

    /// <summary>
    ///     Projects a single Message entity to a MessageResponse DTO for SignalR delivery.
    /// </summary>
    /// <param name="m">The message entity to project.</param>
    /// <returns>A MessageResponse object with message content and metadata.</returns>
    public static MessageResponse SelectMessageResponseFromFullMessage(this Message m)
    {
        return new MessageResponse
        {
            ChatId = m.ChatId,
            Content = m.Content,
            CreatedAt = m.CreatedAt.Value,
            SenderId = m.SenderId,
            IsConfirmationMessage = m.IsConfirmationMessage
        };
    }
}
