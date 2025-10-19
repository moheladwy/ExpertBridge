// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Core.Queries;

/// <summary>
/// Provides extension methods for querying and projecting JobPosting entities.
/// </summary>
/// <remarks>
/// These query extensions enable reusable patterns for loading related data
/// and projecting to response DTOs with user-specific vote and application states.
/// </remarks>
public static class JobPostingQueries
{
    /// <summary>
    /// Eagerly loads all related data for job postings including author, votes, media, comments, applications, and tags.
    /// </summary>
    /// <param name="query">The source queryable of job postings.</param>
    /// <returns>A queryable of job postings with all navigation properties included.</returns>
    /// <remarks>
    /// Uses AsNoTracking for read-only queries. Includes: Author, Votes, Medias, Comments, JobApplications, JobPostingTags with Tags.
    /// </remarks>
    public static IQueryable<JobPosting> FullyPopulatedJobPostingQuery(this IQueryable<JobPosting> query) =>
        query
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Votes)
            .Include(p => p.Medias)
            .Include(p => p.Comments)
            .Include(p => p.JobApplications)
            .Include(p => p.JobPostingTags)
            .ThenInclude(t => t.Tag);

    //.ThenInclude(c => c.Author)
    //.Include(p => p.Comments)
    //.ThenInclude(c => c.Replies)
    //.ThenInclude(c => c.Author)
    /// <summary>
    /// Eagerly loads all related data for job postings and filters by the specified predicate.
    /// </summary>
    /// <param name="query">The source queryable of job postings.</param>
    /// <param name="predicate">The filter expression to apply.</param>
    /// <returns>A filtered queryable with all navigation properties included.</returns>
    public static IQueryable<JobPosting> FullyPopulatedJobPostingQuery(this IQueryable<JobPosting> query,
        Expression<Func<JobPosting, bool>> predicate) =>
        query
            .FullyPopulatedJobPostingQuery()
            .Where(predicate);

    /// <summary>
    /// Projects a queryable of JobPosting entities to JobPostingResponse DTOs with user-specific vote and application information.
    /// </summary>
    /// <param name="query">The source queryable of job postings.</param>
    /// <param name="userProfileId">The ID of the current user for determining vote and application states, or null for anonymous users.</param>
    /// <returns>A queryable of JobPostingResponse objects with vote counts, tags, media, and application status.</returns>
    public static IQueryable<JobPostingResponse> SelectJopPostingResponseFromFullJobPosting(
        this IQueryable<JobPosting> query,
        string? userProfileId) =>
        query
            .Select(p => SelectJopPostingResponseFromFullJobPosting(p, userProfileId));

    /// <summary>
    /// Projects a single JobPosting entity to a JobPostingResponse DTO with user-specific vote and application information.
    /// </summary>
    /// <param name="p">The job posting entity to project.</param>
    /// <param name="userProfileId">The ID of the current user for determining vote and application states, or null for anonymous users.</param>
    /// <returns>A JobPostingResponse object with vote counts, tags, media, whether the user has voted, and whether they have applied.</returns>
    public static JobPostingResponse SelectJopPostingResponseFromFullJobPosting(
        this JobPosting p,
        string? userProfileId) =>
        new()
        {
            IsUpvoted = p.Votes.Any(v => v.IsUpvote && v.ProfileId == userProfileId),
            IsDownvoted = p.Votes.Any(v => !v.IsUpvote && v.ProfileId == userProfileId),
            Title = p.Title,
            Content = p.Content,
            Area = p.Area,
            Budget = p.Budget,
            Language = p.Language,
            Tags = p.JobPostingTags.Select(pt => pt.Tag.SelectTagResponseFromTag()).ToList(),
            Author = p.Author.SelectAuthorResponseFromProfile(),
            CreatedAt = p.CreatedAt.Value,
            LastModified = p.UpdatedAt,
            // RelevanceScore = p.Embedding?.CosineDistance(queryEmbedding) ?? 0,
            Id = p.Id,
            Upvotes = p.Votes.Count(v => v.IsUpvote),
            Downvotes = p.Votes.Count(v => !v.IsUpvote),
            Comments = p.Comments.Count,
            IsAppliedFor = p.JobApplications.Any(ja => ja.ApplicantId == userProfileId),
            Medias = p.Medias.AsQueryable().SelectMediaObjectResponse().ToList()
        };
}
