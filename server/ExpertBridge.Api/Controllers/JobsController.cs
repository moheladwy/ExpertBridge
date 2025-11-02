using ExpertBridge.Api.Services;
using ExpertBridge.Contract.Requests.CreateJobOffer;
using ExpertBridge.Contract.Requests.UpdateJobStatus;
using ExpertBridge.Contract.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     The JobsController class manages job-related operations including retrieving job details,
///     managing job offers, and updating job statuses for the current user.
/// </summary>
/// <remarks>
///     This controller handles multiple endpoints for job-related functionality, such as
///     getting details of a job, listing jobs for the authenticated user, initiating and managing job offers.
///     Most of the endpoints require authorization as they operate on user-specific data.
/// </remarks>
/// <example>
///     The controller provides various endpoints such as:
///     - Retrieving a specific job by its ID.
///     - Listing jobs associated with the logged-in user.
///     - Creating and managing job offers.
/// </example>
/// <seealso cref="ControllerBase" />
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobsController : ControllerBase
{
    /// <summary>
    ///     Provides access to job-related operations and functionalities within the application domain.
    ///     Serves as a service to handle processing and management of job entities.
    /// </summary>
    private readonly JobService _jobService;

    /// <summary>
    ///     Manages user-related operations and functionalities within the application.
    ///     Handles the processing and interaction with user entities.
    /// </summary>
    private readonly UserService _userService;

    /// <summary>
    ///     Handles HTTP requests related to job operations.
    ///     Provides endpoints for managing job-related functionalities,
    ///     such as creating, retrieving, updating, and deleting jobs.
    /// </summary>
    public JobsController(
        UserService userService,
        JobService jobService)
    {
        _userService = userService;
        _jobService = jobService;
    }

    /// <summary>
    ///     Retrieves a job by its unique identifier.
    ///     Fetches the details of a specific job based on the provided job ID.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job to retrieve.</param>
    /// <returns>
    ///     An <see cref="ActionResult{T}" /> containing the job details wrapped in a <see cref="JobResponse" /> object
    ///     upon successful retrieval.
    /// </returns>
    [HttpGet("{jobId}")]
    public async Task<ActionResult<JobResponse>> GetJobById(string jobId)
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var job = await _jobService.GetJobByIdAsync(userProfile, jobId);

        return Ok(job);
    }

    /// <summary>
    ///     Retrieves a list of jobs associated with the currently authenticated user.
    ///     Returns a collection of job responses specific to the user's profile.
    /// </summary>
    /// <returns>
    ///     A collection of job responses associated with the current user wrapped in an HTTP action result.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobResponse>>> GetJobsForCurrentUser()
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var jobs = await _jobService.GetMyJobsAsync(userProfile);

        return Ok(jobs);
    }

    /// <summary>
    ///     Initiates a job offer creation process by processing the provided job offer details
    ///     and creating a new job offer linked to the current user's profile.
    /// </summary>
    /// <param name="request">
    ///     The details of the job offer to be created, encapsulated within
    ///     a <see cref="CreateJobOfferRequest" /> object.
    /// </param>
    /// <returns>
    ///     An <see cref="ActionResult{T}" /> containing a <see cref="JobOfferResponse" /> object
    ///     which represents the result of the job offer creation process.
    /// </returns>
    [HttpPost("offers")]
    public async Task<ActionResult<JobOfferResponse>> InitiateJobOffer(
        [FromBody] CreateJobOfferRequest request)
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var offer = await _jobService.CreateJobOfferAsync(userProfile, request);

        return offer;
    }

    /// <summary>
    ///     Retrieves the list of job offers associated with the current user.
    ///     Returns information about the offers extended to the user.
    /// </summary>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains
    ///     a list of job offer responses associated with the current user.
    /// </returns>
    [HttpGet("offers")]
    public async Task<List<JobOfferResponse>> GetMyOffers()
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var offers = await _jobService.GetMyOffersAsync(userProfile);

        return offers;
    }

    /// <summary>
    ///     Retrieves a list of job offers that the currently authenticated user has received.
    ///     This method fetches the offers associated with the user's profile and returns them as a list.
    /// </summary>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    ///     The task result contains a list of job offer responses.
    /// </returns>
    [HttpGet("offers/received")]
    public async Task<List<JobOfferResponse>> GetReceivedOffers()
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var offers = await _jobService.GetReceivedOffersAsync(userProfile);

        return offers;
    }

    /// <summary>
    ///     Updates the status of a specific job offer identified by its ID.
    ///     The operation validates the user's profile and updates the job offer status
    ///     based on the provided input data.
    /// </summary>
    /// <param name="offerId">
    ///     The unique identifier of the job offer whose status needs to be updated.
    /// </param>
    /// <param name="request">
    ///     An object containing the details of the job status update request.
    /// </param>
    /// <returns>
    ///     A <see cref="JobOfferResponse" /> object that represents the updated status of the job offer.
    /// </returns>
    [HttpPatch("offers/{offerId}/status")]
    public async Task<JobOfferResponse> UpdateJobStatus(
        string offerId, [FromBody] UpdateJobStatusRequest request)
    {
        var user = await _userService.GetCurrentUserProfileOrThrowAsync();

        var offer = await _jobService.UpdateJobOfferStatusAsync(offerId, user, request);

        return offer;
    }

    /// <summary>
    ///     Accepts a job offer associated with the provided offer ID.
    ///     Updates the job status to reflect the acceptance and returns the updated job details.
    /// </summary>
    /// <param name="offerId">
    ///     The unique identifier of the offer to accept.
    /// </param>
    /// <returns>
    ///     A <see cref="JobResponse" /> object containing the details of the accepted job.
    /// </returns>
    [HttpPatch("offers/{offerId}/accept")]
    public async Task<JobResponse> AcceptOffer([FromRoute] string offerId)
    {
        var user = await _userService.GetCurrentUserProfileOrThrowAsync();

        var job = await _jobService.AcceptOfferAsync(user, offerId);

        return job;
    }

    /// <summary>
    ///     Accepts an application for a job.
    ///     Processes the specified application and updates its status to accepted.
    /// </summary>
    /// <param name="applicationId">
    ///     The ID of the application to be accepted.
    /// </param>
    /// <returns>
    ///     A <see cref="JobResponse" /> object containing details of the job associated with the accepted application.
    /// </returns>
    [HttpPatch("applications/{applicationId}/accept")]
    public async Task<JobResponse> AcceptApplication([FromRoute] string applicationId)
    {
        var user = await _userService.GetCurrentUserProfileOrThrowAsync();

        var job = await _jobService.AcceptApplicationAsync(user, applicationId);

        return job;
    }
}
