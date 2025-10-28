// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Entities.Profiles;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Contract.Queries;

/// <summary>
///     Provides extension methods for querying and projecting Profile entities.
/// </summary>
/// <remarks>
///     These query extensions support loading related user data, skills, comments, and votes,
///     and projecting to various response DTOs including ProfileResponse, AuthorResponse, and ApplicantResponse.
/// </remarks>
public static class ProfileQueries
{
    /// <summary>
    ///     Eagerly loads all related data for profiles including user, skills, comments, and votes.
    /// </summary>
    /// <param name="query">The source queryable of profiles.</param>
    /// <returns>A queryable of profiles with all navigation properties included.</returns>
    /// <remarks>
    ///     Uses AsNoTracking for read-only queries. Includes: User, ProfileSkills with Skills, Comments with Votes.
    /// </remarks>
    public static IQueryable<Profile> FullyPopulatedProfileQuery(this IQueryable<Profile> query)
    {
        return query
            .AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.ProfileSkills)
            .ThenInclude(ps => ps.Skill)
            .Include(p => p.Comments)
            .ThenInclude(c => c.Votes);
    }

    /// <summary>
    ///     Eagerly loads all related data for profiles and filters by the specified predicate.
    /// </summary>
    /// <param name="query">The source queryable of profiles.</param>
    /// <param name="predicate">The filter expression to apply.</param>
    /// <returns>A filtered queryable with all navigation properties included.</returns>
    public static IQueryable<Profile> FullyPopulatedProfileQuery(
        this IQueryable<Profile> query,
        Expression<Func<Profile, bool>> predicate)
    {
        return query
            .FullyPopulatedProfileQuery()
            .Where(predicate);
    }

    /// <summary>
    ///     Projects a queryable of Profile entities to ProfileResponse DTOs with calculated reputation.
    /// </summary>
    /// <param name="query">The source queryable of profiles.</param>
    /// <returns>A queryable of ProfileResponse objects with skills, ratings, and reputation scores.</returns>
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
                Reputation = p.SelectReputationFromProfile()
            });
    }

    /// <summary>
    ///     Projects a Profile entity to an AuthorResponse DTO for use in posts and comments.
    /// </summary>
    /// <param name="profile">The profile entity to project, or null.</param>
    /// <returns>An AuthorResponse object with basic profile information, or null if the input is null.</returns>
    public static AuthorResponse? SelectAuthorResponseFromProfile(this Profile? profile)
    {
        return profile == null
            ? null
            : new AuthorResponse
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

    /// <summary>
    ///     Extracts the list of skill names from a profile's ProfileSkills collection.
    /// </summary>
    /// <param name="profile">The profile entity to extract skills from.</param>
    /// <returns>A list of skill names, or an empty list if profile is null or has no skills.</returns>
    private static List<string> SelectSkillsNamesFromProfile(this Profile profile)
    {
        return profile?.ProfileSkills
            .Select(ps => ps.Skill.Name)
            .ToList() ?? [];
    }

    /// <summary>
    ///     Projects a Profile entity to an ApplicantResponse DTO with reputation score for job applications.
    /// </summary>
    /// <param name="profile">The profile entity to project, or null.</param>
    /// <returns>An ApplicantResponse object with profile information and reputation, or null if the input is null.</returns>
    public static ApplicantResponse? SelectApplicantResponseFromProfile(this Profile? profile)
    {
        return profile == null
            ? null
            : new ApplicantResponse
            {
                Id = profile.Id,
                UserId = profile.UserId,
                JobTitle = profile.JobTitle,
                ProfilePictureUrl = profile.ProfilePictureUrl,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Username = profile.Username,
                Reputation = profile.SelectReputationFromProfile()
            };
    }

    /// <summary>
    ///     Calculates the reputation score for a profile based on comment votes.
    /// </summary>
    /// <param name="profile">The profile entity to calculate reputation for.</param>
    /// <returns>The reputation score (upvotes minus downvotes).</returns>
    private static int SelectReputationFromProfile(this Profile profile)
    {
        return profile.Comments.Sum(c => c.Votes.Count(v => v.IsUpvote))
               - profile.Comments.Sum(c => c.Votes.Count(v => !v.IsUpvote));
    }
}
