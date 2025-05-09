using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Requests.Jobs;
using ExpertBridge.Api.Responses;
using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.JobStatuses;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Exceptions;
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
                .FirstOrDefaultAsync(p => p.UserId == clientProfileId);

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