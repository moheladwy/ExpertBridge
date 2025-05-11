using System.Threading.Channels;
using ExpertBridge.Core.Requests;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Settings;
using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpertBridge.Api.DomainServices;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Core.Entities.ManyToManyRelationships.UserInterests;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly AuthorizationHelper _authHelper;
    private readonly ChannelWriter<UserInterestsProsessingMessage> _channelWriter;

    public ProfilesController(
        ExpertBridgeDbContext dbContext,
        AuthorizationHelper authHelper,
        Channel<UserInterestsProsessingMessage> channel)
    {
        _dbContext = dbContext;
        _authHelper = authHelper;
        _channelWriter = channel.Writer;
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

    [Route("/api/v2/{cotroller}/onboard")]
    [HttpPost]
    public async Task<ProfileResponse> OnboardUserV2([FromBody] OnboardUserRequestV2 request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _authHelper.GetCurrentUserAsync();

        if (user is null) throw new UnauthorizedAccessException("The user is not authorized.");

        var existingTags = await _dbContext.Tags
            .AsNoTracking()
            .Where(t =>
                request.Tags.Contains(t.EnglishName) || request.Tags.Contains(t.ArabicName)
                )
            .ToListAsync();

        var existingTagIds = existingTags.Select(t => t.Id).ToList();

        var existingUserInterests = await _dbContext.UserInterests
            .AsNoTracking()
            .Where(ui => ui.ProfileId == user.Profile.Id && existingTagIds.Contains(ui.TagId))
            .Select(ui => ui.TagId)
            .ToListAsync();

        var tagsToBeAddedToUserInterests = existingTagIds
            .Where(tagId => !existingUserInterests.Contains(tagId))
            .ToList();

        await _dbContext.UserInterests.AddRangeAsync(tagsToBeAddedToUserInterests.Select(tagId =>
            new UserInterest { ProfileId = user.Profile.Id, TagId = tagId})
        );

        var newTagsToBeProcessed = request.Tags
            .Where(t => !existingTags.Any(et => et.EnglishName == t || et.ArabicName == t))
            .ToList();
        user.IsOnboarded = true;
        await _dbContext.SaveChangesAsync();

        await _channelWriter.WriteAsync(new UserInterestsProsessingMessage
        {
            UserProfileId = user.Profile.Id,
            InterestsTags = newTagsToBeProcessed
        });

        var response = await _dbContext.Profiles
            .FullyPopulatedProfileQuery(p => p.UserId == user.Id)
            .SelectProfileResponseFromProfile()
            .FirstOrDefaultAsync();

        return response ?? throw new ProfileNotFoundException($"User[{user.Id}] Profile was not found");
    }
}
