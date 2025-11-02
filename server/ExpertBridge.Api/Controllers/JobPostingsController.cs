using ExpertBridge.Application.DomainServices;
using ExpertBridge.Contract.Requests.ApplyToJobPosting;
using ExpertBridge.Contract.Requests.CreateJobPosting;
using ExpertBridge.Contract.Requests.EditJobPosting;
using ExpertBridge.Contract.Requests.JobPostingsPagination;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ExpertBridge.Api.Controllers;

// CURRENT PROGRESS:
// TODO REMOVE PROFILE ID FROM AREA Entity (meaningless)
// CHANGE CoverLetter DTO in apply endpoint to be several variables (price, etc)
// Q: Should job postings be just like announcements with no up/downvotes or should it be like a post
// Currently its like an announcement/posting, can extend later if needed/time permits
/// <summary>
///     Controller that manages operations related to job postings, such as creating, retrieving,
///     editing, upvoting/downvoting, and deleting job postings. It also provides endpoints for
///     suggesting similar jobs, retrieving applications, and allowing users to apply to job postings.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobPostingsController : ControllerBase
{
    /// <summary>
    ///     Responsible for handling operations related to job postings within the application domain.
    ///     Provides functionalities to create, manage, or retrieve job postings data.
    /// </summary>
    private readonly JobPostingService _jobPostingService;

    private readonly ILogger<JobPostingsController> _logger;

    /// <summary>
    ///     Manages user-related operations and functionalities.
    ///     Handles authentication, user management, and user-specific data processing within the application.
    /// </summary>
    private readonly UserService _userService;

    /// <summary>
    ///     The JobPostingsController class handles the HTTP endpoints for managing job postings.
    ///     This includes creating, retrieving, updating, and applying to job postings.
    /// </summary>
    /// <remarks>
    ///     The controller utilizes services for handling job posting and user-related logic, and
    ///     includes logging capabilities. Authorization is required for accessing these endpoints.
    /// </remarks>
    public JobPostingsController(
        JobPostingService jobPostingService,
        UserService userService,
        ILogger<JobPostingsController> logger)
    {
        _jobPostingService = jobPostingService;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    ///     Handles the creation of a new job posting.
    /// </summary>
    /// <param name="request">The request containing the details necessary to create a new job posting.</param>
    /// <returns>
    ///     An ActionResult containing the created job posting details wrapped in a <see cref="JobPostingResponse" /> object.
    /// </returns>
    [HttpPost] // POST /api/JobPostings
    public async Task<ActionResult<JobPostingResponse>> CreateJobPosting(
        [FromBody] CreateJobPostingRequest request)
    {
        var authorProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var response = await _jobPostingService.CreateAsync(request, authorProfile);

        return CreatedAtAction(nameof(GetById), new { postingId = response.Id }, response);
    }

    /// <summary>
    ///     Retrieves a job posting by its unique identifier.
    /// </summary>
    /// <param name="postingId">The unique identifier of the job posting to retrieve.</param>
    /// <returns>
    ///     An <see cref="ActionResult{T}" /> containing the <see cref="JobPostingResponse" /> if the job posting is found.
    ///     If the job posting does not exist, returns a 404 Not Found response with an appropriate error message.
    /// </returns>
    [AllowAnonymous]
    [HttpGet("{postingId}", Name = "GetJobPostingByIdAction")] // Added Name for CreatedAtAction
    public async Task<ActionResult<JobPostingResponse>> GetById([FromRoute] string postingId)
    {
        // string? userProfileId = await _userService.GetCurrentUserProfileIdOrEmptyAsync(); // Get this if needed for vote perspective
        // For now, assuming your UserService provides a method like this from the previous discussion:
        var user = await _userService.GetCurrentUserPopulatedModelAsync(); // Or your specific method
        var userProfileId = user?.Profile.Id;
        var response = await _jobPostingService.GetJobPostingByIdAsync(postingId, userProfileId);

        if (response == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = $"Post with id={postingId} was not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return response;
    }

    /// <summary>
    ///     Retrieves a list of job postings similar to the specified job posting.
    /// </summary>
    /// <param name="postingId">The unique identifier of the job posting to find similar jobs for.</param>
    /// <param name="limit">The maximum number of similar jobs to retrieve. Defaults to 5 if not provided.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of job postings that are similar to the specified job posting.</returns>
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

    /// <summary>
    ///     Retrieves a list of suggested job postings for the current user.
    ///     The suggestions are based on the user's profile and preferences.
    /// </summary>
    /// <param name="limit">The maximum number of suggested job postings to retrieve. If null, a default value of 5 is used.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to signal the request to cancel the operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation, wrapping a list of <see cref="SimilarJobsResponse" />
    ///     objects.
    /// </returns>
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


    /// <summary>
    ///     Retrieves a paginated list of job postings based on an offset-based pagination model.
    ///     This endpoint is designed to serve a feed of job postings, optionally tailored to user preferences if
    ///     authenticated.
    /// </summary>
    /// <param name="request">
    ///     The pagination request containing details such as offset, limit, and other optional filtering
    ///     parameters.
    /// </param>
    /// <returns>
    ///     An asynchronous task that resolves to an ActionResult containing a paginated response of job postings.
    /// </returns>
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

    /// <summary>
    ///     Updates an existing job posting with new details provided by the user.
    ///     The user must have the appropriate authorization to edit a job posting.
    /// </summary>
    /// <param name="postingId">The unique identifier of the job posting to be edited.</param>
    /// <param name="request">An object containing the updated information for the job posting.</param>
    /// <returns>A response containing the updated details of the job posting.</returns>
    [HttpPatch("{postingId}")]
    public async Task<ActionResult<JobPostingResponse>> Edit(
        [FromRoute] string postingId,
        [FromBody] EditJobPostingRequest request)
    {
        var editorProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var postResponse = await _jobPostingService.EditJopPostingAsync(postingId, request, editorProfile);

        return postResponse;
    }

    /// <summary>
    ///     Handles the upvoting functionality for a specific job posting.
    ///     This method allows an authenticated user to upvote a job posting.
    /// </summary>
    /// <param name="postingId">The unique identifier of the job posting to upvote.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains an
    ///     <see cref="JobPostingResponse" /> object with the status and details of the updated job posting.
    /// </returns>
    [HttpPatch("{postingId}/upvote")]
    public async Task<ActionResult<JobPostingResponse>> Upvote([FromRoute] string postingId)
    {
        var voterProfile =
            await _userService.GetCurrentUserProfileOrThrowAsync(); // Ensures authenticated user with profile

        var postResponse = await _jobPostingService
            .VoteJobPostingAsync(postingId, voterProfile, true); // true for upvote

        return postResponse;
    }

    /// <summary>
    ///     Handles the downvote action for a job posting.
    ///     This endpoint allows an authenticated user to cast a downvote on a specific job posting.
    /// </summary>
    /// <param name="postingId">The unique identifier of the job posting to downvote.</param>
    /// <returns>
    ///     Returns a <see cref="JobPostingResponse" /> containing the updated details of the job posting
    ///     after the downvote action has been processed.
    /// </returns>
    [HttpPatch("{postingId}/downvote")]
    public async Task<ActionResult<JobPostingResponse>> Downvote([FromRoute] string postingId)
    {
        var voterProfile =
            await _userService.GetCurrentUserProfileOrThrowAsync(); // Ensures authenticated user with profile

        var postResponse = await _jobPostingService
            .VoteJobPostingAsync(postingId, voterProfile, false); // false for downvote

        return postResponse;
    }

    /// <summary>
    ///     Deletes an existing job posting identified by the provided posting ID.
    ///     This operation is idempotent, returning a success status even if the resource
    ///     does not exist or the user is not permitted to delete it.
    /// </summary>
    /// <param name="postingId">The unique identifier of the job posting to delete.</param>
    /// <returns>
    ///     An IActionResult indicating the result of the delete operation. If successful, it returns NoContent (HTTP
    ///     204).
    /// </returns>
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
            // BEWARE!
            // In an HTTP DELETE, you always want to return 204 no content.
            // No matter what happens. The only exception is if the auth middleware
            // refused the request from the beginning, else you do not return anything
            // other than no content.
            // https://stackoverflow.com/questions/6439416/status-code-when-deleting-a-resource-using-http-delete-for-the-second-time#comment33002038_6440374

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

    /// <summary>
    ///     Handles the application process of a user to a specific job posting by utilizing the
    ///     provided job posting ID and application request details.
    /// </summary>
    /// <param name="jobPostingId">
    ///     The unique identifier of the job posting to which the user is applying.
    /// </param>
    /// <param name="request">
    ///     The request object containing details related to the application, such as user-provided
    ///     data like cover letter, price expectations, or additional information required.
    /// </param>
    /// <returns>
    ///     Returns an IActionResult indicating the outcome of the application process. Typically,
    ///     this will be an HTTP status response.
    /// </returns>
    [HttpPost("{jobPostingId}/apply")]
    public async Task<IActionResult> ApplyToJobPosting(
        [FromRoute] string jobPostingId,
        [FromBody] ApplyToJobPostingRequest request)
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var response = await _jobPostingService.ApplyToJobPostingAsync(jobPostingId, userProfile, request);

        return Ok();
    }

    /// <summary>
    ///     Retrieves a list of job applications associated with a specific job posting.
    /// </summary>
    /// <param name="jobPostingId">The unique identifier of the job posting for which to retrieve applications.</param>
    /// <returns>A task that resolves to a list of job application responses for the given job posting.</returns>
    [HttpGet("{jobPostingId}/applications")]
    public async Task<List<JobApplicationResponse>> GetJobApplications([FromRoute] string jobPostingId)
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var applications = await _jobPostingService.GetJobApplicationsAsync(jobPostingId, userProfile);

        return applications;
    }
}
