using ExpertBridge.Application.DomainServices;
using ExpertBridge.Contract.Requests.ApplyToJobPosting;
using ExpertBridge.Contract.Requests.CreateJobPosting;
using ExpertBridge.Contract.Requests.EditJobPosting;
using ExpertBridge.Contract.Requests.JobPostingsPagination;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Contract.Requests.ApplyToJobPosting;
using ExpertBridge.Contract.Requests.CreateJobPosting;
using ExpertBridge.Contract.Requests.EditJobPosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ExpertBridge.Api.Controllers;

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
    private readonly JobPostingService _jobPostingService;
    private readonly ILogger<JobPostingsController> _logger;
    private readonly UserService _userService;

    public JobPostingsController(
        JobPostingService jobPostingService,
        UserService userService,
        ILogger<JobPostingsController> logger)
    {
        _jobPostingService = jobPostingService;
        _userService = userService;
        _logger = logger;
    }

    // POST /api/JobPostings
    [HttpPost]
    public async Task<ActionResult<JobPostingResponse>> CreateJobPosting(
        [FromBody] CreateJobPostingRequest request)
    {
        var authorProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var response = await _jobPostingService.CreateAsync(request, authorProfile);

        return CreatedAtAction(nameof(GetById), new { postingId = response.Id }, response);
    }

    [AllowAnonymous]
    [HttpGet("{postingId}", Name = "GetJobPostingByIdAction")] // Added Name for CreatedAtAction
    public async Task<ActionResult<JobPostingResponse>> GetById([FromRoute] string postingId)
    {
        // string? userProfileId = await _userService.GetCurrentUserProfileIdOrEmptyAsync(); // Get this if needed for vote perspective
        // For now, assuming your UserService provides a method like this from previous discussion:
        var user = await _userService.GetCurrentUserPopulatedModelAsync(); // Or your specific method
        var userProfileId = user?.Profile?.Id;


        var response = await _jobPostingService.GetJobPostingByIdAsync(postingId, userProfileId);

        if (response == null)
        {
            // return post ?? throw new PostNotFoundException($"Post with id={postId} was not found");
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = $"Post with id={postingId} was not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return response;
    }

    [AllowAnonymous]
    [HttpGet("{postingId}/similar")]
    public async Task<List<SimilarJobsResponse>> GetSimilarJobs(
        [FromRoute] string postingId,
        [FromQuery] int? limit = 5,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(postingId, nameof(postingId)); // Service handles

        var similarJobs = await _jobPostingService.GetSimilarJobsAsync(
            postingId,
            limit ?? 5, // Default to 5 if not provided
            cancellationToken);

        return similarJobs;
    }

    [AllowAnonymous]
    [HttpGet("suggested")]
    public async Task<ActionResult<List<SimilarJobsResponse>>> GetSuggestedJobs(
        [FromQuery] int? limit,
        CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetCurrentUserPopulatedModelAsync();
        var suggestedJobs = await _jobPostingService.GetSuggestedJobsAsync(
            user?.Profile,
            limit ?? 5, // Default to 5 if not provided
            cancellationToken);

        return suggestedJobs;
    }


    [AllowAnonymous]
    [HttpPost("feed")]
    [ResponseCache(NoStore = true)]
    public async Task<ActionResult<JobPostingsPaginatedResponse>> GetOffsetPaginated(
        [FromBody] JobPostingsPaginationRequest request)
    {
        var user = await _userService.GetCurrentUserPopulatedModelAsync();
        var jobs = await _jobPostingService.GetRecommendedJobsOffsetPageAsync(user?.Profile, request);

        return jobs;
    }

    [HttpPatch("{postingId}")]
    public async Task<ActionResult<JobPostingResponse>> Edit(
        [FromRoute] string postingId,
        [FromBody] EditJobPostingRequest request)
    {
        var editorProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var postResponse = await _jobPostingService.EditJopPostingAsync(postingId, request, editorProfile);

        return postResponse;
    }

    [HttpPatch("{postingId}/upvote")]
    public async Task<ActionResult<JobPostingResponse>> Upvote([FromRoute] string postingId)
    {
        var voterProfile =
            await _userService.GetCurrentUserProfileOrThrowAsync(); // Ensures authenticated user with profile

        var postResponse = await _jobPostingService
            .VoteJobPostingAsync(postingId, voterProfile, true); // true for upvote

        return postResponse;
    }

    [HttpPatch("{postingId}/downvote")]
    public async Task<ActionResult<JobPostingResponse>> Downvote([FromRoute] string postingId)
    {
        var voterProfile =
            await _userService.GetCurrentUserProfileOrThrowAsync(); // Ensures authenticated user with profile

        var postResponse = await _jobPostingService
            .VoteJobPostingAsync(postingId, voterProfile, false); // false for downvote

        return postResponse;
    }

    [HttpDelete("{postingId}")]
    public async Task<IActionResult> Delete([FromRoute] string postingId)
    {
        var deleterProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        try
        {
            await _jobPostingService.DeleteJobPostingAsync(postingId, deleterProfile);
            return NoContent(); // Always 204 for DELETE success (even if resource was already gone)
        }
        catch (ForbiddenAccessException ex)
        {
            // Even for forbidden, you might return 204 to not leak info,
            // or 403 if you want to be explicit. Standard is often 204 for DELETE.
            // However, if _userService.GetCurrentUserProfileOrThrowAsync() throws Unauthorized,
            // that will result in 401 before this.
            // A 403 here means authenticated user, but not permitted for *this specific resource*.
            // For DELETE, many prefer to still return 204 to not reveal existence/non-existence.
            // But if it's a clear "you can't do that" to an owned resource, 403 is also fine.
            // Let's stick to 204 for simplicity and common DELETE idempotency interpretation.
            Log.Warning(ex,
                "Forbidden attempt to delete post {PostId} by user {UserProfileId}",
                postingId, deleterProfile.Id);

            return NoContent();
        }
    }

    // BEWARE!
    // In an HTTP DELETE, you always want to return 204 no content.
    // No matter what happens. The only exception is if the auth middleware
    // refused the request from the beginning, else you do not return anything
    // other than no content.
    // https://stackoverflow.com/questions/6439416/status-code-when-deleting-a-resource-using-http-delete-for-the-second-time#comment33002038_6440374


    [HttpPost("{jobPostingId}/apply")]
    public async Task<IActionResult> ApplyToJobPosting(
        [FromRoute] string jobPostingId,
        [FromBody] ApplyToJobPostingRequest request)
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var response = await _jobPostingService.ApplyToJobPostingAsync(jobPostingId, userProfile, request);

        return Ok();
    }

    [HttpGet("{jobPostingId}/applications")]
    public async Task<List<JobApplicationResponse>> GetJobApplications([FromRoute] string jobPostingId)
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var applications = await _jobPostingService.GetJobApplicationsAsync(jobPostingId, userProfile);

        return applications;
    }
}
