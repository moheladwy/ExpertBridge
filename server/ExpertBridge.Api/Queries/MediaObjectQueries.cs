// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.Media;
using ExpertBridge.Api.Responses;
using ExpertBridge.Api.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ExpertBridge.Api.Queries
{
    public static class MediaObjectQueries
    {
        // WARNING!
        // This method uses a hardcoded URL for the media object.
        // This is a magic string that will be a pain in the ass to maintain
        // and cause dependent modules to rebuild on change. 
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
}
