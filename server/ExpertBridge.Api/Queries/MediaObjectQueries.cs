// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.Media;
using ExpertBridge.Api.Responses;

namespace ExpertBridge.Api.Queries
{
    public static class MediaObjectQueries
    {
        public static IQueryable<MediaObjectResponse> SelectMediaObjectResponse(
            this IQueryable<MediaObject> query)
        {
            return query
                .Select(m => new MediaObjectResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Url = m.Url,
                    Type = m.Type,
                    Key = m.Key
                });
        }
    }
}
