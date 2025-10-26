using ExpertBridge.Application.DomainServices;
using ExpertBridge.Core.Requests.Jobs;
using ExpertBridge.Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobsController : ControllerBase
{
    private readonly JobService _jobService;
    private readonly UserService _userService;

    public JobsController(
        UserService userService,
        JobService jobService)
    {
        _userService = userService;
        _jobService = jobService;
    }

    [HttpGet("{jobId}")]
    public async Task<ActionResult<JobResponse>> GetJobById(string jobId)
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var job = await _jobService.GetJobByIdAsync(userProfile, jobId);

        return Ok(job);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobResponse>>> GetJobsForCurrentUser()
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var jobs = await _jobService.GetMyJobsAsync(userProfile);

        return Ok(jobs);
    }

    [HttpPost("offers")]
    public async Task<ActionResult<JobOfferResponse>> InitiateJobOffer(
        [FromBody] CreateJobOfferRequest request)
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var offer = await _jobService.CreateJobOfferAsync(userProfile, request);

        return offer;
    }

    [HttpGet("offers")]
    public async Task<List<JobOfferResponse>> GetMyOffers()
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var offers = await _jobService.GetMyOffersAsync(userProfile);

        return offers;
    }

    [HttpGet("offers/received")]
    public async Task<List<JobOfferResponse>> GetReceivedOffers()
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var offers = await _jobService.GetReceivedOffersAsync(userProfile);

        return offers;
    }

    [HttpPatch("offers/{offerId}/status")]
    public async Task<JobOfferResponse> UpdateJobStatus(
        string offerId, [FromBody] UpdateJobStatusRequest request)
    {
        var user = await _userService.GetCurrentUserProfileOrThrowAsync();

        var offer = await _jobService.UpdateJobOfferStatusAsync(offerId, user, request);

        return offer;
    }

    [HttpPatch("offers/{offerId}/accept")]
    public async Task<JobResponse> AcceptOffer([FromRoute] string offerId)
    {
        var user = await _userService.GetCurrentUserProfileOrThrowAsync();

        var job = await _jobService.AcceptOfferAsync(user, offerId);

        return job;
    }

    [HttpPatch("applications/{applicationId}/accept")]
    public async Task<JobResponse> AcceptApplication([FromRoute] string applicationId)
    {
        var user = await _userService.GetCurrentUserProfileOrThrowAsync();

        var job = await _jobService.AcceptApplicationAsync(user, applicationId);

        return job;
    }
}
