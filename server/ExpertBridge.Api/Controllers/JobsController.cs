using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Requests.Jobs;
using ExpertBridge.Api.Responses;
using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.JobStatuses;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Requests.Jobs;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ExpertBridge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobsController : ControllerBase
    {
        private readonly ExpertBridgeDbContext _dbContext;
        private readonly AuthorizationHelper _authHelper;

        public JobsController(
            ExpertBridgeDbContext dbContext,
            AuthorizationHelper authHelper)
        {
            _dbContext = dbContext;
            _authHelper = authHelper;
        }

        [HttpPost("offer")]
        public async Task<ActionResult<JobResponse>> InitiateJobOffer([FromBody] InitiateJobOfferRequest request)
        {

            var user = await _authHelper.GetCurrentUserAsync();
            var clientProfileId = user?.Profile?.Id ?? string.Empty;


            if (string.IsNullOrEmpty(clientProfileId))
            {
                return Unauthorized("Profile ID could not be determined.");
            }

            var clientProfile = await _dbContext.Profiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == clientProfileId);

            if (clientProfile == null)
            {
                throw new ProfileNotFoundException("Client profile not found.");
            }

            var contractorProfile = await _dbContext.Profiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == request.ContractorProfileId);

            if (contractorProfile == null)
            {
                throw new ProfileNotFoundException("Contractor profile not found.");
            }

            if (clientProfile.Id == contractorProfile.Id)
            {
                return BadRequest("Cannot initiate a job offer to yourself.");
            }

            // if JobPostingId is valid if provided
            if (!string.IsNullOrEmpty(request.JobPostingId))
            {
                var jobPostingExists = await _dbContext.JobPostings.AnyAsync(jp => jp.Id == request.JobPostingId && jp.AuthorId == clientProfile.Id);
                if (!jobPostingExists)
                {
                    return BadRequest("Invalid JobPostingId / it does not belong to you.");
                }
            }

            var newJob = new Job
            {
                Title = request.Title,
                Description = request.Description,
                ActualCost = request.ProposedRate,
                AuthorId = clientProfile.Id,
                WorkerId = contractorProfile.Id,
                Status = JobStatusEnum.Offered, // initial status
                JobPostingId = request.JobPostingId,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Jobs.Add(newJob);
            await _dbContext.SaveChangesAsync();



            return Ok(MapToJobResponse(newJob, clientProfile, contractorProfile));
        }

        [HttpPatch("{jobId}/response")]
        public async Task<ActionResult<JobResponse>> RespondToJobOffer(string jobId, [FromBody] RespondToJobOfferRequest request)
        {
            var user = await _authHelper.GetCurrentUserAsync();
            if (user?.Profile == null){
                return Unauthorized("User profile not found.");
            }

            var contractorProfileId = user.Profile.Id;

            var job = await _dbContext.Jobs
                .Include(j => j.Author)
                    .ThenInclude(p => p.User)
                .Include(j => j.Worker)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
            {
                throw new JobNotFoundException($"Job with Id '{jobId} is not found.'");
            }

            if (job.WorkerId != contractorProfileId)
            {
                return Forbid("You are not authorised to respond to this job request");
            }

            if (job.Status != JobStatusEnum.Offered)
            {
                return BadRequest("This job is no longer in the offered state.");
            }

            job.UpdatedAt = DateTime.UtcNow;

            if (request.Accept)
            {
                job.Status = JobStatusEnum.Accepted;
                job.StartedAt = DateTime.UtcNow;
            } else {
                job.Status = JobStatusEnum.Declined;
            }

            _dbContext.Jobs.Update(job);
            await _dbContext.SaveChangesAsync();

            return Ok(MapToJobResponse(job, job.Author, job.Worker));

        }

        [HttpPatch("{jobId}/status")]
        public async Task<ActionResult<JobResponse>> UpdateJobStatus(string jobId, [FromBody] UpdateJobStatusRequest request)
        {
            var currentUser = await _authHelper.GetCurrentUserAsync();
            if (currentUser?.Profile == null)
            {
                return Unauthorized("User profile not found.");
            }

            var userProfileId = currentUser.Profile.Id;

            var job = await _dbContext.Jobs
                .Include(j => j.Author)
                    .ThenInclude(p => p.User)
                .Include(j => j.Worker)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null) 
            {
                throw new JobNotFoundException($"Job with '{jobId} not found.");
            }
            
            // user is either client or contractor
            if (job.AuthorId != userProfileId && job.WorkerId != userProfileId)
            {
                return Forbid("You are not authorised to update the status of this job.");
            }

            // business logic
            // only client can move from PendingClientApproval to Completed
            // only contractor can move from accepted to inprogress when they arrives
            // only contractor cam move from inprogress to PendingClientApproval
            // Either can be moved to cancelled if not completed

            var oldStatus = job.Status;
            if (oldStatus == request.NewStatus)
            {
                return BadRequest($"Job is already in '{request.NewStatus}' status.");
            }

            var newStatus = request.NewStatus;
            bool isValidTransition = false;

            if (oldStatus==JobStatusEnum.PendingClientApproval && newStatus==JobStatusEnum.Completed && job.AuthorId==userProfileId
                || oldStatus==JobStatusEnum.Accepted && newStatus==JobStatusEnum.InProgress && job.WorkerId==userProfileId
                || oldStatus==JobStatusEnum.InProgress && newStatus==JobStatusEnum.PendingClientApproval && job.WorkerId==userProfileId
                || !(oldStatus==JobStatusEnum.Completed) && newStatus==JobStatusEnum.Cancelled){
                isValidTransition = true;
            }

            if (!isValidTransition)
            {
                return BadRequest($"Invalid status transition form '{oldStatus}' to '{request.NewStatus}' or you're not authorized.");
            }

            job.Status = request.NewStatus;
            job.UpdatedAt = DateTime.UtcNow;

            if (request.NewStatus == JobStatusEnum.Completed)
            {
                job.EndedAt = DateTime.UtcNow;
            }

            _dbContext.Jobs.Update(job);
            await _dbContext.SaveChangesAsync();

            return Ok(MapToJobResponse(job, job.Author, job.Worker));
        }
        
        [HttpGet("{jobId}")]
        public async Task<ActionResult<JobResponse>> GetJobById(string jobId)
        {
            var currentUser = await _authHelper.GetCurrentUserAsync();
            if (currentUser?.Profile == null)
            {
                return Unauthorized("User profile not found.");
            }

            var userProfileId = currentUser.Profile.Id;

            var job = await _dbContext.Jobs
                .Include(j => j.Author)
                    .ThenInclude(p => p.User)
                .Include(j => j.Worker)
                    .ThenInclude(p => p.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job==null)
            {
                throw new JobNotFoundException("Job Id is not found");
            }

            if (job.AuthorId!=userProfileId && job.WorkerId!=userProfileId)
            {
                return Forbid("You are not authorized to view this job.");
            }

            return Ok(MapToJobResponse(job, job.Author, job.Worker));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobResponse>>> GetJobsForCurrentUser()
        {
            var currentUser = await _authHelper.GetCurrentUserAsync();
            if (currentUser?.Profile == null)
            {
                return Unauthorized("User profile not found");
            }

            var userProfileId = currentUser.Profile.Id;

            var jobs = await _dbContext.Jobs
                .Include(j => j.Author)
                    .ThenInclude(p => p.User)
                .Include(j => j.Worker)
                    .ThenInclude(p => p.User)
                .Where(j => j.AuthorId == userProfileId || j.WorkerId == userProfileId)
                .AsNoTracking()
                .ToListAsync();

            if (!jobs.Any())
            {
                return Ok(new List<JobResponse>());
            }

            var jobResponses = jobs.Select(job => MapToJobResponse(job, job.Author, job.Worker)).ToList();

            return Ok(jobResponses);
        }
        
        
        private static JobResponse MapToJobResponse(Job job, Profile authorProfile, Profile workerProfile)
        {
            return new JobResponse
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Status = job.Status.ToString(),
                ActualCost = job.ActualCost,
                StartedAt = job.StartedAt,
                EndedAt = job.EndedAt,
                JobPostingId = job.JobPostingId,
                CreatedAt = job.CreatedAt,
                UpdatedAt = job.UpdatedAt,
                AuthorProfile = new ProfileSummaryResponse
                {
                    ProfileId = authorProfile.Id,
                    FirstName = authorProfile.User?.FirstName,
                    LastName = authorProfile.User?.LastName,
                    ProfilePictureUrl = authorProfile.ProfilePictureUrl,
                },
                WorkerProfile = new ProfileSummaryResponse
                {
                    ProfileId = workerProfile.Id,
                    FirstName = workerProfile.User?.FirstName,
                    LastName = workerProfile.User?.LastName,
                    ProfilePictureUrl = workerProfile.ProfilePictureUrl,
                }
            };
        }  
    }

}
