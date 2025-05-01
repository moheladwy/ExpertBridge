using System.Threading.Channels;
using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Api.Queries;
using ExpertBridge.Api.Services;
using ExpertBridge.Api.Settings;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Requests;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly AuthorizationHelper _authHelper;

    public ProfilesController(ExpertBridgeDbContext dbContext, AuthorizationHelper authHelper)
    {
        _dbContext = dbContext;
        _authHelper = authHelper;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ProfileResponse> GetProfile()
    {
        var user = await _authHelper.GetCurrentUserAsync();
        if (user == null) throw new UnauthorizedGetMyProfileException();

        var profile = await _dbContext.Profiles
            .FullyPopulatedProfileQuery(p => p.UserId == user.Id)
            .SelectProfileResponseFromProfile()
            .FirstOrDefaultAsync();

        if (profile == null)
            throw new ProfileNotFoundException($"User[{user.Id}] Profile was not found");

        return profile;
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    [ResponseCache(CacheProfileName = CacheProfiles.Default)]
    public async Task<ProfileResponse> GetProfile(string id)
    {
        var profile = await _dbContext.Profiles
            .FullyPopulatedProfileQuery(profile => profile.Id == id)
            .SelectProfileResponseFromProfile()
            .FirstOrDefaultAsync();

        if (profile == null)
            throw new ProfileNotFoundException($"Profile with id={id} was not found");

        return profile;
    }

    [HttpPost("onboard")]
    public async Task<ProfileResponse> OnboardUser(
        [FromBody] OnboardUserRequest request,
        [FromServices] TaggingService _taggingService)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _authHelper.GetCurrentUserAsync();

        user.IsOnboarded = true;

        await _taggingService.AddTagsToUserProfileAsync(user.Profile.Id, request.TagIds);

        var response = await _dbContext.Profiles
            .FullyPopulatedProfileQuery(p => p.UserId == user.Id)
            .SelectProfileResponseFromProfile()
            .FirstOrDefaultAsync();

        return response;
    }
}
