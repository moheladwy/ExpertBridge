// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobOffers;
using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.JobStatuses;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Requests.Jobs;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.DomainServices
{
    public class JobService
    {
        private readonly ExpertBridgeDbContext _dbContext;
        private readonly NotificationFacade _notifications;
        private readonly ILogger<JobService> _logger;

        public JobService(
            ExpertBridgeDbContext dbContext,
            NotificationFacade notifications,
            ILogger<JobService> logger)
        {
            _dbContext = dbContext;
            _notifications = notifications;
            _logger = logger;
        }

        public async Task<JobOfferResponse> CreateJobOfferAsync(
            Profile clientProfile,
            CreateJobOfferRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var contractorProfile = await _dbContext.Profiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == request.WorkerId);

            if (contractorProfile == null)
            {
                throw new ProfileNotFoundException("Contractor profile not found.");
            }

            if (clientProfile.Id == contractorProfile.Id)
            {
                throw new BadHttpRequestException("Cannot initiate a job offer to yourself.");
            }

            var existingOffer = await _dbContext.JobOffers
                .AnyAsync(o =>
                    o.AuthorId == clientProfile.Id
                    && o.WorkerId == contractorProfile.Id
                    && o.Title == request.Title.Trim());

            if (existingOffer)
            {
                throw new BadHttpRequestException("You have already made an offer for this job.");
            }

            var offer = new JobOffer
            {
                Title = request.Title,
                Description = request.Description,
                Budget = request.Budget,
                Area = request.Area,
                AuthorId = clientProfile.Id,
                WorkerId = contractorProfile.Id,
                Author = clientProfile,
                Worker = contractorProfile
            };

            await _dbContext.JobOffers.AddAsync(offer);
            await _dbContext.SaveChangesAsync();

            await _notifications.NotifyJobOfferCreatedAsync(offer);

            return offer.SelectJobOfferResponseFromEntity();
        }

        public async Task<List<JobOfferResponse>> GetMyOffersAsync(Profile userProfile)
        {
            var offers = await _dbContext.JobOffers
                .Include(o => o.Worker)
                .Where(o => o.AuthorId == userProfile.Id)
                .SelectJobOfferResponseFromEntity()
                .ToListAsync();

            return offers;
        }

        public async Task<List<JobOfferResponse>> GetReceivedOffersAsync(Profile userProfile)
        {
            var offers = await _dbContext.JobOffers
                .Include(o => o.Author)
                .Where(o => o.WorkerId == userProfile.Id)
                .SelectJobOfferResponseFromEntity()
                .ToListAsync();

            return offers;
        }
    }
}
