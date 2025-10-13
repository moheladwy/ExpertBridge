// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Tags;
using ExpertBridge.Core.Responses;

namespace ExpertBridge.Core.Queries;

public static class TagQueries
{
    public static IQueryable<TagResponse> SelectTagResponseFromTag(this IQueryable<Tag> query) =>
        query
            .Select(t => SelectTagResponseFromTag(t));

    public static TagResponse SelectTagResponseFromTag(this Tag t) =>
        new() { TagId = t.Id, EnglishName = t.EnglishName, ArabicName = t.ArabicName, Description = t.Description };
}
