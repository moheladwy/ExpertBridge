using ExpertBridge.Core.Entities.Chats;
using ExpertBridge.Core.Entities.JobOffers;
using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Requests.Jobs;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpertBridge.Application.DomainServices
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

        public async Task<List<JobResponse>> GetMyJobsAsync(Profile userProfile)
        {
            var jobs = await _dbContext.Jobs
                .Include(j => j.Worker)
                .Include(j => j.Author)
                .Where(j => j.AuthorId == userProfile.Id || j.WorkerId == userProfile.Id)
                .SelectJobResponseFromFullJob()
                .ToListAsync();

            return jobs;
        }

        public async Task<JobResponse> GetJobByIdAsync(Profile userProfile, string jobId)
        {
            var job = await _dbContext.Jobs
                .Include(j => j.Worker)
                .Include(j => j.Author)
                .Where(j => j.Id == jobId
                            && (j.AuthorId == userProfile.Id || j.WorkerId == userProfile.Id))
                .SelectJobResponseFromFullJob()
                .FirstOrDefaultAsync();

            if (job == null)
            {
                throw new JobNotFoundException();
            }

            return job;
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
                .Where(o => o.AuthorId == userProfile.Id
                            && !o.IsDeclined && !o.IsAccepted)
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

        public async Task<JobOfferResponse> UpdateJobOfferStatusAsync(
            string jobOfferId,
            Profile worker,
            UpdateJobStatusRequest request)
        {
            ArgumentException.ThrowIfNullOrEmpty(jobOfferId);

            var offer = await _dbContext.JobOffers
                .FirstOrDefaultAsync(o => o.Id == jobOfferId && o.WorkerId == worker.Id);

            if (offer == null)
            {
                throw new JobOfferNotFoundException("Job offer not found.");
            }

            var accept = request.Status == "accepted";

            offer.IsAccepted = accept;
            offer.IsDeclined = !accept;

            await _dbContext.SaveChangesAsync();

            return offer.SelectJobOfferResponseFromEntity();
        }

        public async Task<JobResponse> AcceptOfferAsync(
            Profile workerProfile,
            string offerId)
        {
            ArgumentNullException.ThrowIfNull(workerProfile);

            var offer = await _dbContext.JobOffers
                .Include(o => o.Author)
                .Include(o => o.Worker)
                .FirstOrDefaultAsync(o => o.Id == offerId && o.WorkerId == workerProfile.Id);

            if (offer == null)
            {
                throw new JobOfferNotFoundException($"Job offer id={offerId} not found or you are not the worker.");
            }

            return await CreateJobInternalAsync(new Job
            {
                Area = offer.Area,
                AuthorId = offer.AuthorId,
                Author = offer.Author,
                Worker = offer.Worker,
                Description = offer.Description,
                WorkerId = offer.WorkerId,
                Title = offer.Title,
                ActualCost = offer.Budget,
                StartedAt = DateTime.UtcNow,
            });
        }

        public async Task<JobResponse> AcceptApplicationAsync(
            Profile hirerProfile,
            string applicationId)
        {
            ArgumentNullException.ThrowIfNull(hirerProfile);

            var application = await _dbContext.JobApplications
                .Include(a => a.JobPosting)
                .Include(a => a.Applicant)
                .FirstOrDefaultAsync(a => a.Id == applicationId && a.JobPosting.AuthorId == hirerProfile.Id);

            if (application == null)
            {
                throw new JobApplicationNotFoundException($"Job application id={applicationId} not found or you are not the hirer.");
            }

            return await CreateJobInternalAsync(new Job
            {
                Area = application.JobPosting.Area,
                AuthorId = hirerProfile.Id,
                Description = application.JobPosting.Content,
                WorkerId = application.ApplicantId,
                Worker = application.Applicant,
                Author = hirerProfile,
                Title = application.JobPosting.Title,
                ActualCost = application.JobPosting.Budget,
                StartedAt = DateTime.UtcNow,
            });
        }

        private async Task<JobResponse> CreateJobInternalAsync(
            Job job)
        {
            var chat = new Chat
            {
                HirerId = job.AuthorId,
                WorkerId = job.WorkerId,
            };



            await _dbContext.Chats.AddAsync(chat);

            job.Chat = chat;

            await _dbContext.Jobs.AddAsync(job);

            await _dbContext.SaveChangesAsync();

            return job.SelectJobResponseFromFullJob();
        }
    }
}
