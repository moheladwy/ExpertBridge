using ExpertBridge.Api.Helpers;
using ExpertBridge.Core.Entities.JobApplication;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Requests.JobPosting;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    // CURRENT PROGRESS:
    // TODO REMOVE PROFILE ID FROM AREA Entity (meaningless)
    // CHANGE CoverLetter DTO in apply endpoint to be several variables (price, etc)
    // Q: Should job postings be just like announcements with no up/downvotes or should it be like a post
    // Currently its like an announcement/posting, can extend later if needed/time permits
    public class JobPostingsController : ControllerBase
    {
        private readonly ExpertBridgeDbContext _dbContext;
        private readonly AuthorizationHelper _authHelper;

        public JobPostingsController(ExpertBridgeDbContext dbContext, AuthorizationHelper authHelper)
        {
            _dbContext = dbContext;
            _authHelper = authHelper;
        }

        [HttpPost("{jobPostingId}/apply")]
        public async Task<IActionResult> ApplyToJobPosting(string jobPostingId, [FromBody] ApplyToJobPostingRequest request)
        {
            var user = await _authHelper.GetCurrentUserAsync();
            var contractorProfileId = user?.Profile?.Id;
            if (contractorProfileId == null)
                return Unauthorized("User profile not found.");

            var jobPosting = await _dbContext.JobPostings.FindAsync(jobPostingId);
            if (jobPosting == null)
                return NotFound("Job posting not found.");

            var alreadyApplied = await _dbContext.JobApplications
                .AnyAsync(a => a.JobPostingId == jobPostingId && a.ContractorProfileId == contractorProfileId);
            if (alreadyApplied)
                return BadRequest("You have already applied to this job posting.");

            var application = new JobApplication
            {
                JobPostingId = jobPostingId,
                ContractorProfileId = contractorProfileId,
                CoverLetter = request.CoverLetter
            };

            _dbContext.JobApplications.Add(application);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{jobPostingId}/applicants")]
        public async Task<IActionResult> GetApplicants(string jobPostingId)
        {
            var user = await _authHelper.GetCurrentUserAsync();
            var clientProfileId = user?.Profile?.Id;
            if (clientProfileId == null)
                return Unauthorized("User profile not found.");

            var jobPosting = await _dbContext.JobPostings
                .AsNoTracking()
                .FirstOrDefaultAsync(jp => jp.Id == jobPostingId && jp.AuthorId == clientProfileId);

            if (jobPosting == null)
                return NotFound("Job posting not found or you are not the author.");

            var applicants = await _dbContext.JobApplications
                .Where(a => a.JobPostingId == jobPostingId)
                .Include(a => a.ContractorProfile)
                    .ThenInclude(p => p.User)
                .Select(a => new
                {
                    a.Id,
                    a.ContractorProfileId,
                    a.CoverLetter,
                    a.AppliedAt,
                    Contractor = new
                    {
                        a.ContractorProfile.Id,
                        a.ContractorProfile.User.FirstName,
                        a.ContractorProfile.User.LastName,
                        a.ContractorProfile.ProfilePictureUrl
                    }
                })
                .ToListAsync();

            return Ok(applicants);
        }


        // POST /api/JobPostings
        [HttpPost]
        public async Task<ActionResult<JobPostingResponse>> CreateJobPosting([FromBody] CreateJobPostingRequest request)
        {
            var user = await _authHelper.GetCurrentUserAsync();
            var authorProfileId = user?.Profile?.Id;
            if (authorProfileId == null)
                return Unauthorized("User profile not found.");

            var jobPosting = new JobPosting
            {
                Id = Guid.NewGuid().ToString(),
                AuthorId = authorProfileId,
                AreaId = request.AreaId,
                CategoryId = request.CategoryId,
                Title = request.Title,
                Description = request.Description,
                Cost = request.Cost,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.JobPostings.Add(jobPosting);
            await _dbContext.SaveChangesAsync();

            //  including navigation properties just incase
            var author = await _dbContext.Profiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == authorProfileId);

            var response = new JobPostingResponse
            {
                Id = jobPosting.Id,
                Title = jobPosting.Title,
                Description = jobPosting.Description,
                AreaId = jobPosting.AreaId,
                CategoryId = jobPosting.CategoryId,
                Cost = jobPosting.Cost,
                CreatedAt = jobPosting.CreatedAt ?? DateTime.MinValue,
                UpdatedAt = jobPosting.UpdatedAt ?? DateTime.MinValue,
                AuthorProfile = author != null ? new ProfileSummaryResponse
                {
                    ProfileId = author.Id,
                    FirstName = author.User?.FirstName,
                    LastName = author.User?.LastName,
                    ProfilePictureUrl = author.ProfilePictureUrl
                } : null
            };

            return Ok(response);
        }

        // GET /api/JobPostings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPostingResponse>>> GetAllJobPostings()
        {
            var jobPostings = await _dbContext.JobPostings
                .Include(jp => jp.Author)
                    .ThenInclude(p => p.User)
                .AsNoTracking()
                .ToListAsync();

            var responses = jobPostings.Select(jp => new JobPostingResponse
            {
                Id = jp.Id,
                Title = jp.Title,
                Description = jp.Description,
                AreaId = jp.AreaId,
                CategoryId = jp.CategoryId,
                Cost = jp.Cost,
                CreatedAt = jp.CreatedAt ?? DateTime.MinValue,
                UpdatedAt = jp.UpdatedAt ?? DateTime.MinValue,
                AuthorProfile = jp.Author != null ? new ProfileSummaryResponse
                {
                    ProfileId = jp.Author.Id,
                    FirstName = jp.Author.User?.FirstName,
                    LastName = jp.Author.User?.LastName,
                    ProfilePictureUrl = jp.Author.ProfilePictureUrl
                } : null
            }).ToList();

            return Ok(responses);
        }



    }


}