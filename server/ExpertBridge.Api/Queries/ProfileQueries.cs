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
                ProfilePictureUrl = profile.ProfilePictureUrl,
                CreatedAt = profile.CreatedAt,
                Email = profile.Email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                IsBanned = profile.IsBanned,
                PhoneNumber = profile.PhoneNumber,
                Rating = profile.Rating,
                RatingCount = profile.RatingCount,
                Username = profile.Username
            };
        }
    }
}
