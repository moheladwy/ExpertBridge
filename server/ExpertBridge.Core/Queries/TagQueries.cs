// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertBridge.Core.Entities.Tags;
using ExpertBridge.Core.Responses;

namespace ExpertBridge.Core.Queries
{
    public static class TagQueries
    {
        public static IQueryable<TagResponse> SelectTagResponseFromTag(this IQueryable<Tag> query)
        {
            return query
                .Select(t => SelectTagResponseFromTag(t));
        }

        public static TagResponse SelectTagResponseFromTag(this Tag t)
        {
            return new TagResponse
            {
                TagId = t.Id,
                EnglishName = t.EnglishName,
                ArabicName = t.ArabicName,
                Description = t.Description
            };
        }
    }
}
