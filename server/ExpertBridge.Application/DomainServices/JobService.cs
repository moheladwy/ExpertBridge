using ExpertBridge.Core.Entities.Chats;
using ExpertBridge.Core.Entities.JobOffers;
using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Requests.CreateJobOffer;
using ExpertBridge.Core.Requests.UpdateJobStatus;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpertBridge.Application.DomainServices;

/// <summary>
/// Provides comprehensive job lifecycle management including offers, applications, and job execution tracking.
/// </summary>
/// <remarks>
/// This service orchestrates the complete job workflow from initial offer through completion,
/// managing the state transitions between job postings, applications, offers, and active jobs.
/// 
/// **Job Lifecycle States:**
/// 
/// **1. Job Posting → Job Application:**
/// - Client posts job requirement (JobPostingService)
/// - Contractors apply with cover letter and bid
/// - Client reviews applications
/// - Client accepts application → Creates Job + Chat
/// 
/// **2. Job Offer (Direct Approach):**
/// - Client sends direct offer to contractor
/// - Contractor accepts/declines offer
/// - Acceptance → Creates Job + Chat
/// 
/// **3. Active Job:**
/// - Job record created with budget, timeline
/// - Private chat opened for coordination
/// - Work progresses with updates in chat
/// - Completion marked, payment processed
/// 
/// **Architecture Integration:**
/// - JobPostingService: Manages job postings and applications
/// - MessagingService: Handles chat communications
/// - NotificationFacade: Sends real-time notifications
/// - Chat: Created automatically when job accepted
/// 
/// **Database Relationships:**
/// <code>
/// JobPosting (1) → (N) JobApplication
/// JobApplication (1) → (1) Job (when accepted)
/// JobOffer (1) → (1) Job (when accepted)
/// Job (1) → (1) Chat (private conversation)
/// Job (N) → (1) Profile (Author/Hirer)
/// Job (N) → (1) Profile (Worker/Contractor)
/// </code>
/// 
/// **Key Flows:**
/// 
/// **Application Acceptance:**
/// <code>
/// Client → AcceptApplicationAsync(applicationId)
///   ↓
/// Create Job entity
///   ↓
/// Create Chat entity
///   ↓
/// Notify contractor
///   ↓
/// Return JobResponse
/// </code>
/// 
/// **Offer Acceptance:**
/// <code>
/// Contractor → AcceptOfferAsync(offerId)
///   ↓
/// Verify offer ownership
///   ↓
/// Create Job + Chat
///   ↓
/// Notify client
///   ↓
/// Return JobResponse
/// </code>
/// 
/// **Security Patterns:**
/// - Offers: Only worker can accept their own offers
/// - Applications: Only job posting author can accept applications
/// - Jobs: Only participants (hirer/worker) can access job details
/// - Self-offers prevented (cannot offer job to yourself)
/// 
/// **Notification Integration:**
/// - Job offer created → Notify contractor
/// - Offer accepted → Notify client
/// - Application accepted → Notify contractor
/// - Job milestones → Notify both parties
/// 
/// **Future Enhancements:**
/// - Escrow payment integration
/// - Milestone tracking and partial payments
/// - Job completion verification
/// - Rating and review system
/// - Dispute resolution workflow
/// - Job cancellation with refunds
/// 
/// Registered as scoped service with per-request lifetime.
/// </remarks>
public class JobService
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly ILogger<JobService> _logger;
    private readonly NotificationFacade _notifications;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for job and related entity operations.</param>
    /// <param name="notifications">The notification facade for real-time alerts.</param>
    /// <param name="logger">The logger for diagnostic information.</param>
    public JobService(
        ExpertBridgeDbContext dbContext,
        NotificationFacade notifications,
        ILogger<JobService> logger)
    {
        _dbContext = dbContext;
        _notifications = notifications;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all jobs where the user is either the client (hirer) or the contractor (worker).
    /// </summary>
    /// <param name="userProfile">The profile of the user requesting their jobs.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a list of <see cref="JobResponse"/> 
    /// objects for all jobs where the user is a participant.
    /// </returns>
    /// <remarks>
    /// **Query Scope:**
    /// Returns jobs where userProfile.Id matches either:
    /// - Job.AuthorId (user is the client/hirer)
    /// - Job.WorkerId (user is the contractor/worker)
    /// 
    /// **Use Cases:**
    /// - "My Jobs" dashboard showing active and completed work
    /// - Income/expense tracking
    /// - Job history and portfolio building
    /// 
    /// **Example:**
    /// <code>
    /// [HttpGet("my-jobs")]
    /// public async Task&lt;IActionResult&gt; GetMyJobs()
    /// {
    ///     var profile = await _userService.GetCurrentUserProfileOrThrowAsync();
    ///     var jobs = await _jobService.GetMyJobsAsync(profile);
    ///     return Ok(jobs);
    /// }
    /// </code>
    /// </remarks>
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

    /// <summary>
    /// Retrieves a specific job by ID if the user is a participant (hirer or worker).
    /// </summary>
    /// <param name="userProfile">The profile of the user requesting the job details.</param>
    /// <param name="jobId">The unique identifier of the job to retrieve.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the <see cref="JobResponse"/>.
    /// </returns>
    /// <exception cref="JobNotFoundException">
    /// Thrown when the job doesn't exist or the user is not authorized to access it.
    /// </exception>
    /// <remarks>
    /// **Authorization:**
    /// User must be either the hirer (AuthorId) or worker (WorkerId) to access job details.
    /// 
    /// **Included Data:**
    /// - Job details (title, budget, dates)
    /// - Worker profile information
    /// - Author/client profile information
    /// - Chat reference for communication
    /// </remarks>
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

    /// <summary>
    /// Creates a direct job offer from a client to a specific contractor.
    /// </summary>
    /// <param name="clientProfile">The profile of the client making the offer.</param>
    /// <param name="request">The job offer details including contractor ID, budget, and description.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the created <see cref="JobOfferResponse"/>.
    /// </returns>
    /// <exception cref="ProfileNotFoundException">Thrown when the contractor profile is not found.</exception>
    /// <exception cref="BadHttpRequestException">
    /// Thrown when trying to offer a job to yourself or duplicate offer exists.
    /// </exception>
    /// <remarks>
    /// **Direct Offer Flow:**
    /// Client → CreateJobOfferAsync → Contractor receives notification → Contractor accepts/declines
    /// 
    /// **Validation:**
    /// - Contractor profile must exist
    /// - Cannot offer job to yourself
    /// - Cannot create duplicate offers (same client, worker, title)
    /// 
    /// **Business Rules:**
    /// - Offer remains pending until contractor responds
    /// - Client can create multiple offers to different contractors
    /// - Offers are separate from job posting applications
    /// 
    /// **Notification:**
    /// Contractor receives real-time notification of new offer.
    /// 
    /// **Example:**
    /// <code>
    /// var offer = await _jobService.CreateJobOfferAsync(clientProfile, new CreateJobOfferRequest
    /// {
    ///     WorkerId = contractorId,
    ///     Title = "Website Redesign",
    ///     Description = "Modernize company website",
    ///     Budget = 5000,
    ///     Area = "Web Development"
    /// });
    /// </code>
    /// </remarks>
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

    /// <summary>
    /// Retrieves all pending job offers created by the user (where user is the client).
    /// </summary>
    /// <param name="userProfile">The profile of the user (client) who created the offers.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a list of pending <see cref="JobOfferResponse"/> objects.
    /// </returns>
    /// <remarks>
    /// **Filter Criteria:**
    /// - AuthorId = userProfile.Id (offers created by this user)
    /// - IsDeclined = false (not declined)
    /// - IsAccepted = false (not accepted)
    /// 
    /// **Use Case:**
    /// Displays offers awaiting contractor response in client's dashboard.
    /// </remarks>
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

    /// <summary>
    /// Retrieves all job offers received by the user (where user is the contractor).
    /// </summary>
    /// <param name="userProfile">The profile of the user (contractor) receiving the offers.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a list of <see cref="JobOfferResponse"/> objects
    /// including accepted, declined, and pending offers.
    /// </returns>
    /// <remarks>
    /// **Filter Criteria:**
    /// - WorkerId = userProfile.Id (offers sent to this contractor)
    /// - Includes all statuses (pending, accepted, declined)
    /// 
    /// **Use Case:**
    /// Shows all job opportunities offered to the contractor.
    /// </remarks>
    public async Task<List<JobOfferResponse>> GetReceivedOffersAsync(Profile userProfile)
    {
        var offers = await _dbContext.JobOffers
            .Include(o => o.Author)
            .Where(o => o.WorkerId == userProfile.Id)
            .SelectJobOfferResponseFromEntity()
            .ToListAsync();

        return offers;
    }

    /// <summary>
    /// Updates the status of a job offer (accept or decline) by the contractor.
    /// </summary>
    /// <param name="jobOfferId">The unique identifier of the job offer.</param>
    /// <param name="worker">The profile of the contractor responding to the offer.</param>
    /// <param name="request">The status update request containing "accepted" or other status.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the updated <see cref="JobOfferResponse"/>.
    /// </returns>
    /// <exception cref="JobOfferNotFoundException">
    /// Thrown when the offer doesn't exist or the worker is not the intended recipient.
    /// </exception>
    /// <remarks>
    /// **Status Logic:**
    /// - "accepted": Sets IsAccepted=true, IsDeclined=false
    /// - Other: Sets IsAccepted=false, IsDeclined=true
    /// 
    /// **Authorization:**
    /// Only the intended worker (WorkerId) can update the offer status.
    /// 
    /// **Note:**
    /// This method only updates status flags. Use AcceptOfferAsync to create actual Job entity.
    /// </remarks>
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

    /// <summary>
    /// Accepts a job offer and creates an active Job with associated Chat for coordination.
    /// </summary>
    /// <param name="workerProfile">The profile of the contractor accepting the offer.</param>
    /// <param name="offerId">The unique identifier of the offer to accept.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the created <see cref="JobResponse"/>.
    /// </returns>
    /// <exception cref="JobOfferNotFoundException">
    /// Thrown when the offer doesn't exist or the worker is not the intended recipient.
    /// </exception>
    /// <remarks>
    /// **Acceptance Flow:**
    /// 1. Validate offer exists and worker is recipient
    /// 2. Load offer with Author and Worker profiles
    /// 3. Create Job entity from offer details
    /// 4. Create Chat for client-contractor communication
    /// 5. Save to database atomically
    /// 6. Return JobResponse
    /// 
    /// **Created Entities:**
    /// - Job: Active work agreement
    /// - Chat: Private conversation channel
    /// 
    /// **Job Properties:**
    /// - Title, Description from JobOffer
    /// - Budget becomes ActualCost
    /// - StartedAt = DateTime.UtcNow
    /// - Author/Worker from offer
    /// 
    /// **Post-Acceptance:**
    /// - Offer status should be updated separately
    /// - Notifications sent to client
    /// - Chat available in MessagingService
    /// </remarks>
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
            StartedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Accepts a job application and creates an active Job with associated Chat.
    /// </summary>
    /// <param name="hirerProfile">The profile of the client (job posting author) accepting the application.</param>
    /// <param name="applicationId">The unique identifier of the job application to accept.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the created <see cref="JobResponse"/>.
    /// </returns>
    /// <exception cref="JobApplicationNotFoundException">
    /// Thrown when the application doesn't exist or the user is not the job posting author.
    /// </exception>
    /// <remarks>
    /// **Application Acceptance Flow:**
    /// 1. Validate application exists and user is job posting author
    /// 2. Load application with JobPosting and Applicant
    /// 3. Create Job from application/posting details
    /// 4. Create Chat for coordination
    /// 5. Save atomically
    /// 
    /// **Authorization:**
    /// Only the job posting author (hirer) can accept applications.
    /// 
    /// **Job Creation:**
    /// - Title, Description from JobPosting
    /// - Budget from JobPosting.Budget
    /// - Worker from Application.ApplicantId
    /// - Author is hirerProfile
    /// 
    /// **Post-Acceptance:**
    /// - Application should be marked as accepted
    /// - Other applications should be notified/declined
    /// - Applicant receives acceptance notification
    /// </remarks>
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
            throw new JobApplicationNotFoundException(
                $"Job application id={applicationId} not found or you are not the hirer.");
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
            StartedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Internal helper method that creates a Job entity and associated Chat atomically.
    /// </summary>
    /// <param name="job">The partially constructed Job entity to save.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the created <see cref="JobResponse"/>.
    /// </returns>
    /// <remarks>
    /// **Atomic Operations:**
    /// 1. Create Chat (HirerId, WorkerId)
    /// 2. Associate Job with Chat
    /// 3. Save both entities together
    /// 
    /// **Why Separate Method:**
    /// - Reused by AcceptOfferAsync and AcceptApplicationAsync
    /// - Ensures consistent Job + Chat creation pattern
    /// - Single transaction for data consistency
    /// 
    /// **Job Initialization:**
    /// Caller provides:
    /// - Title, Description, Area, Budget/ActualCost
    /// - AuthorId (hirer), WorkerId (contractor)
    /// - Author/Worker navigation properties
    /// 
    /// **Chat Setup:**
    /// - HirerId = Job.AuthorId (client)
    /// - WorkerId = Job.WorkerId (contractor)
    /// - Chat.JobId set automatically by EF relationship
    /// 
    /// This method is private as it should only be called internally after proper validation.
    /// </remarks>
    private async Task<JobResponse> CreateJobInternalAsync(
        Job job)
    {
        var chat = new Chat { HirerId = job.AuthorId, WorkerId = job.WorkerId };


        await _dbContext.Chats.AddAsync(chat);

        job.Chat = chat;

        await _dbContext.Jobs.AddAsync(job);

        await _dbContext.SaveChangesAsync();

        return job.SelectJobResponseFromFullJob();
    }
}
