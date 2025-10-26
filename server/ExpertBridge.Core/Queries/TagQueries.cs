// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Tags;
using ExpertBridge.Core.Responses;

namespace ExpertBridge.Core.Queries;

/// <summary>
///     Provides extension methods for querying and projecting Tag entities.
/// </summary>
/// <remarks>
///     These query extensions project multilingual tags to response DTOs for content categorization.
/// </remarks>
public static class TagQueries
{
    /// <summary>
    ///     Projects a queryable of Tag entities to TagResponse DTOs with multilingual names.
    /// </summary>
    /// <param name="query">The source queryable of tags.</param>
    /// <returns>A queryable of TagResponse objects with English and Arabic translations.</returns>
    public static IQueryable<TagResponse> SelectTagResponseFromTag(this IQueryable<Tag> query)
    {
        return query
            .Select(t => SelectTagResponseFromTag(t));
    }

    /// <summary>
    ///     Projects a single Tag entity to a TagResponse DTO with multilingual names.
    /// </summary>
    /// <param name="t">The tag entity to project.</param>
    /// <returns>A TagResponse object with English and Arabic translations.</returns>
    public static TagResponse SelectTagResponseFromTag(this Tag t)
    {
        return new TagResponse
        {
            TagId = t.Id, EnglishName = t.EnglishName, ArabicName = t.ArabicName, Description = t.Description
        };
    }
}
