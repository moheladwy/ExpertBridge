// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.Messages;
using ExpertBridge.Core.Responses;

namespace ExpertBridge.Core.Queries
{
    public static class MessageQueries
    {
        public static IQueryable<MessageResponse> SelectMessageResponseFromFullMessage(this IQueryable<Message> query)
        {
            return query
                .Select(m => SelectMessageResponseFromFullMessage(m));
        }

        public static MessageResponse SelectMessageResponseFromFullMessage(this Message m)
        {
            return new MessageResponse
            {
                ChatId = m.ChatId,
                Content = m.Content,
                CreatedAt = m.CreatedAt.Value,
                SenderId = m.SenderId,
                IsConfirmationMessage = m.IsConfirmationMessage,
            };
        }
    }
}
