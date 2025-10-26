// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Media;
using ExpertBridge.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Core.Queries;

/// <summary>
/// Provides extension methods for querying and projecting MediaObject entities.
/// </summary>
/// <remarks>
/// These query extensions project media attachments to response DTOs with constructed S3 URLs.
/// </remarks>
public static class MediaObjectQueries
{
    /// <summary>
    /// Projects media objects to MediaObjectResponse DTOs with S3 URLs.
    /// </summary>
    /// <param name="query">The source queryable of media objects.</param>
    /// <returns>A queryable of MediaObjectResponse with constructed URLs.</returns>
    /// <remarks>
    /// <strong>Warning:</strong> This method uses a hardcoded S3 bucket URL.
    /// Consider moving this to configuration to improve maintainability.
    /// </remarks>
    public static IQueryable<MediaObjectResponse> SelectMediaObjectResponse(
        this IQueryable<MediaObject> query)
    {
        return query
            .AsNoTracking()
            .Select(m => new MediaObjectResponse
            {
                Id = m.Id,
                Name = m.Name,
                Type = m.Type,
                Url = $"https://expert-bridge-media.s3.amazonaws.com/{m.Key}"
            });
    }
}
