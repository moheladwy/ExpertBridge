// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.Profiles;
using ExpertBridge.Api.Responses;

namespace ExpertBridge.Api.Queries
{
    public static class ProfileQueries
    {
        public static IQueryable<ProfileResponse> SelectProfileResponseFromProfile(this IQueryable<Profile> query)
        {
            return query
                .Select(p => new ProfileResponse
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    CreatedAt = p.CreatedAt,
                    Email = p.Email,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    IsBanned = p.IsBanned,
                    JobTitle = p.JobTitle,
                    PhoneNumber = p.PhoneNumber,
                    ProfilePictureUrl = p.ProfilePictureUrl,
                    Rating = p.Rating,
                    RatingCount = p.RatingCount,
                    Username = p.Username,
                });                
        }

        public static AuthorResponse? SelectAuthorResponseFromProfile(this Profile? profile)
        {
            return profile == null ? null : new AuthorResponse
            {
                Id = profile.Id,
                UserId = profile.UserId,
                JobTitle = profile.JobTitle,
                ProfilePictureUrl = profile.ProfilePictureUrl,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Username = profile.Username
            };
        }
    }
}
