// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertBridge.Core.Entities.Chats;

namespace ExpertBridge.Core.Queries
{
    public static class ChatQueries
    {
        public static IQueryable<Chat> WhereProfileIsChatParticipant(this IQueryable<Chat> query, string profileId)
        {
            return query.Where(c => c.WorkerId == profileId || c.HirerId == profileId);
        }
    }
}
