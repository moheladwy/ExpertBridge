// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Core.Queries
{
    public static class ProfileQueries
    {
        public static IQueryable<Profile> FullyPopulatedProfileQuery(this IQueryable<Profile> query)
        {
            return query
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.ProfileSkills)
                .ThenInclude(ps => ps.Skill)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Votes)
                ;
        }

        public static IQueryable<Profile> FullyPopulatedProfileQuery(
            this IQueryable<Profile> query,
            Expression<Func<Profile, bool>> predicate)
        {
            return query
                .FullyPopulatedProfileQuery()
                .Where(predicate);
        }

        public static IQueryable<ProfileResponse> SelectProfileResponseFromProfile(this IQueryable<Profile> query)
        {
            return query
                .Select(p => new ProfileResponse
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    CreatedAt = p.CreatedAt.Value,
                    Email = p.Email,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    IsBanned = p.IsBanned,
                    JobTitle = p.JobTitle,
                    Bio = p.Bio,
                    PhoneNumber = p.PhoneNumber,
                    ProfilePictureUrl = p.ProfilePictureUrl,
                    Rating = p.Rating,
                    RatingCount = p.RatingCount,
                    Username = p.Username,
                    IsOnboarded = p.User.IsOnboarded,
                    Skills = p.SelectSkillsNamesFromProfile(),
                    CommentsUpvotes = p.Comments.Sum(c => c.Votes.Count(v => v.IsUpvote)),
                    CommentsDownvotes = p.Comments.Sum(c => c.Votes.Count(v => !v.IsUpvote)),
                    Reputation = p.SelectReputationFromProfile(),
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

        private static List<string> SelectSkillsNamesFromProfile(this Profile profile)
        {
            return profile?.ProfileSkills
                .Select(ps => ps.Skill.Name)
                .ToList() ?? [];
        }

        public static ApplicantResponse? SelectApplicantResponseFromProfile(this Profile? profile)
        {
            return profile == null ? null : new ApplicantResponse
            {
                Id = profile.Id,
                UserId = profile.UserId,
                JobTitle = profile.JobTitle,
                ProfilePictureUrl = profile.ProfilePictureUrl,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Username = profile.Username,
                Reputation = profile.SelectReputationFromProfile(),
            };
        }

        private static int SelectReputationFromProfile(this Profile profile)
        {
            return profile.Comments.Sum(c => c.Votes.Count(v => v.IsUpvote))
                   - profile.Comments.Sum(c => c.Votes.Count(v => !v.IsUpvote));
        }
    }
}
