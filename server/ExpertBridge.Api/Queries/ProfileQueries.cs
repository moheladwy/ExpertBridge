// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.Profiles;
using ExpertBridge.Api.Responses;

namespace ExpertBridge.Api.Queries
{
    public static class ProfileQueries
    {
        public static AuthorResponse? SelectAuthorResponseFromProfile(this Profile? profile)
        {
            return profile == null ? null : new AuthorResponse
            {
                Id = profile.Id,
                UserId = profile.UserId,
                JobTitle = profile.JobTitle,
                ProfilePictureUrl = profile.ProfilePictureUrl
            };
        }
    }
}
