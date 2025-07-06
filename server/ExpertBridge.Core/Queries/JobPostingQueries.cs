// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Responses;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace ExpertBridge.Core.Queries
{
    public static class JobPostingQueries
    {
        public static IQueryable<JobPosting> FullyPopulatedJobPostingQuery(this IQueryable<JobPosting> query)
        {
            return query
                .AsNoTracking()
                .Include(p => p.Author)
                .Include(p => p.Votes)
                .Include(p => p.Medias)
                .Include(p => p.Comments)
                .Include(p => p.JobApplications)
                .Include(p => p.JobPostingTags)
                .ThenInclude(t => t.Tag)
                //.ThenInclude(c => c.Author)
                //.Include(p => p.Comments)
                //.ThenInclude(c => c.Replies)
                //.ThenInclude(c => c.Author)
                ;
        }

        public static IQueryable<JobPosting> FullyPopulatedJobPostingQuery(this IQueryable<JobPosting> query,
            Expression<Func<JobPosting, bool>> predicate)
        {
            return query
                .FullyPopulatedJobPostingQuery()
                .Where(predicate);
        }

        public static IQueryable<JobPostingResponse> SelectJopPostingResponseFromFullJobPosting(
            this IQueryable<JobPosting> query,
            string? userProfileId,
            Vector? queryEmbedding = null)

        {
            return query
                .Select(p => SelectJopPostingResponseFromFullJobPosting(p, userProfileId, queryEmbedding));
        }

        public static JobPostingResponse SelectJopPostingResponseFromFullJobPosting(
            this JobPosting p,
            string? userProfileId,
            Vector? queryEmbedding = null)
        {
            return new JobPostingResponse
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
                RelevanceScore = p.Embedding?.CosineDistance(queryEmbedding) ?? 0,
                Id = p.Id,
                Upvotes = p.Votes.Count(v => v.IsUpvote),
                Downvotes = p.Votes.Count(v => !v.IsUpvote),
                Comments = p.Comments.Count,
                IsAppliedFor = p.JobApplications.Any(ja => ja.ApplicantId == userProfileId),
                Medias = p.Medias.AsQueryable().SelectMediaObjectResponse().ToList(),
            };
        }
    }
}
