// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Messages;
using ExpertBridge.Core.Responses;

namespace ExpertBridge.Core.Queries;

public static class MessageQueries
{
    public static IQueryable<MessageResponse> SelectMessageResponseFromFullMessage(this IQueryable<Message> query) =>
        query
            .Select(m => SelectMessageResponseFromFullMessage(m));

    public static MessageResponse SelectMessageResponseFromFullMessage(this Message m) =>
        new()
        {
            ChatId = m.ChatId,
            Content = m.Content,
            CreatedAt = m.CreatedAt.Value,
            SenderId = m.SenderId,
            IsConfirmationMessage = m.IsConfirmationMessage
        };
}
