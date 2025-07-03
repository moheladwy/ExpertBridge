// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.DataGenerator;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace ExpertBridge.Api.DomainServices
{
    public class ProfileService
    {
        private readonly ExpertBridgeDbContext _dbContext;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(
            ExpertBridgeDbContext dbContext,
            ILogger<ProfileService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<ProfileResponse>> GetSimilarProfilesAsync(
            Profile? userProfile,
            int limit,
            CancellationToken cancellationToken = default)
        {
            var userEmbedding = userProfile?.UserInterestEmbedding ?? Generator.GenerateRandomVector(1024);

            var query = _dbContext.Profiles
                .AsNoTracking()
                .FullyPopulatedProfileQuery()
                .AsQueryable();

            if (userProfile != null)
            {
                query = query.Where(p => p.Id != userProfile.Id);
            }

            var suggested = await query
                .Where(p => p.UserInterestEmbedding != null)
                .Take(limit)
                .OrderBy(p => p.UserInterestEmbedding.CosineDistance(userEmbedding))
                .SelectProfileResponseFromProfile()
                .ToListAsync(cancellationToken);

            return suggested;
        }

        public async Task<List<ProfileResponse>> GetTopReputationProfilesAsync(
            Profile? userProfile,
            int limit,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Profiles
                .AsNoTracking()
                .FullyPopulatedProfileQuery()
                .AsQueryable();

            if (userProfile != null)
            {
                query = query.Where(p => p.Id != userProfile.Id);
            }

            var topProfiles = await query
                //.OrderByDescending(p =>
                //    p.Comments.Sum(c => c.Votes.Count(v => v.IsUpvote))
                //    - p.Comments.Sum(c => c.Votes.Count(v => !v.IsUpvote)))
                .Take(limit)
                .SelectProfileResponseFromProfile()
                .OrderByDescending(p => p.Reputation)
                .ToListAsync(cancellationToken);

            return topProfiles;
        }
    }
}
