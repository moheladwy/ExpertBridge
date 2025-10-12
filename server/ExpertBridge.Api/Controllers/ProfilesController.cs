using System.Threading.Channels;
using ExpertBridge.Application.DomainServices;
using ExpertBridge.Application.Helpers;
using ExpertBridge.Application.Settings;
using ExpertBridge.Contract.Messages;
using ExpertBridge.Core.Entities.ManyToManyRelationships.UserInterests;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Requests;
using ExpertBridge.Core.Requests.UpdateProfileRequest;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using FluentValidation;
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
    private readonly UserService _userService;
    private readonly ProfileService _profileService;
    private readonly ChannelWriter<UserInterestsProsessingMessage> _channelWriter;
    private readonly IValidator<UpdateProfileRequest> _updateProfileRequestValidator;

    public ProfilesController(
        ExpertBridgeDbContext dbContext,
        AuthorizationHelper authHelper,
        Channel<UserInterestsProsessingMessage> channel,
        IValidator<UpdateProfileRequest> updateProfileRequestValidator,
        UserService userService,
        ProfileService profileService)
    {
        _dbContext = dbContext;
        _authHelper = authHelper;
        _channelWriter = channel.Writer;
        _updateProfileRequestValidator = updateProfileRequestValidator;
        _userService = userService;
        _profileService = profileService;
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
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _authHelper.GetCurrentUserAsync();
        if (user is null) throw new UnauthorizedAccessException("The user is not authorized.");

        var existingTags = await _dbContext.Tags
            .AsNoTracking()
            .Where(t =>
                request.Tags.Contains(t.EnglishName) || request.Tags.Contains(t.ArabicName)
                )
            .ToListAsync(cancellationToken);

        var existingTagIds = existingTags.Select(t => t.Id).ToList();

        var existingUserInterests = await _dbContext.UserInterests
            .AsNoTracking()
            .Where(ui => ui.ProfileId == user.Profile.Id && existingTagIds.Contains(ui.TagId))
            .Select(ui => ui.TagId)
            .ToListAsync(cancellationToken);

        var tagsToBeAddedToUserInterests = existingTagIds
            .Where(tagId => !existingUserInterests.Contains(tagId))
            .ToList();

        await _dbContext.UserInterests.AddRangeAsync(tagsToBeAddedToUserInterests.Select(tagId =>
            new UserInterest { ProfileId = user.Profile.Id, TagId = tagId })
        , cancellationToken);

        var newTagsToBeProcessed = request.Tags
            .Where(t => !existingTags.Any(et => et.EnglishName == t || et.ArabicName == t))
            .ToList();
        user.IsOnboarded = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _channelWriter.WriteAsync(new UserInterestsProsessingMessage
        {
            UserProfileId = user.Profile.Id,
            InterestsTags = newTagsToBeProcessed
        }, cancellationToken);

        var response = await _dbContext.Profiles
            .FullyPopulatedProfileQuery(p => p.UserId == user.Id)
            .SelectProfileResponseFromProfile()
            .FirstOrDefaultAsync(cancellationToken);

        return response ?? throw new ProfileNotFoundException($"User[{user.Id}] Profile was not found");
    }

    [Authorize]
    [HttpPut]
    public async Task<ProfileResponse> UpdateProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userService.GetCurrentUserPopulatedModelAsync();
        if (user is null)
            throw new UnauthorizedAccessException("The user is not authorized.");

        var profileResponse = await _profileService.UpdateProfileAsync(
            user,
            request,
            cancellationToken);

        return profileResponse;
    }

    [Authorize]
    [HttpGet("is-username-available/{username}")]
    public async Task<bool> IsUsernameAvailable(
            [FromRoute] string username,
            CancellationToken cancellationToken = default)
    {
        var user = await _authHelper.GetCurrentUserAsync();
        if (user is null)
            throw new UnauthorizedAccessException("The user is not authorized.");

        ArgumentException.ThrowIfNullOrEmpty(username, nameof(username));
        ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));

        // Check if the username is the same as the current user's username
        // if so, return false since it's already taken by the current user.
        if (user.Profile.Username == username)
            return false;

        return !await _dbContext.Profiles
            .AsNoTracking()
            .AnyAsync(
                    p => p.Username == username,
                    cancellationToken
            );
    }

    [AllowAnonymous]
    [HttpGet("suggested")]
    public async Task<List<ProfileResponse>> GetSuggestedProfiles(
        [FromQuery] int? limit,
        CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetCurrentUserPopulatedModelAsync();

        var profiles = await _profileService.GetSimilarProfilesAsync(
            user?.Profile,
            limit ?? 5,
            cancellationToken);

        return profiles;
    }

    [AllowAnonymous]
    [HttpGet("top-reputation")]
    public async Task<List<ProfileResponse>> GetTopReputationProfiles(
        [FromQuery] int? limit,
        CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetCurrentUserPopulatedModelAsync();

        var profiles = await _profileService.GetTopReputationProfilesAsync(
            user?.Profile,
            limit ?? 5,
            cancellationToken);

        return profiles;
    }

    [Authorize]
    [HttpGet("skills")]
    public async Task<List<string>> GetCurrentUserSkills(CancellationToken cancellationToken = default)
    {
        // Current authenticated user.
        var user = await _userService.GetCurrentUserPopulatedModelAsync();
        if (user is null)
            throw new UnauthorizedAccessException("The user is not authorized.");

        var skills = await _dbContext.ProfileSkills
            .Include(ps => ps.Skill)
            .Where(ps => ps.ProfileId == user.Profile.Id)
            .Select(ps => ps.Skill.Name)
            .ToListAsync(cancellationToken);

        return skills;
    }

    [AllowAnonymous]
    [HttpGet("{profileId}/skills")]
    public async Task<List<string>> GetProfileSkills(
        [FromRoute] string profileId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(profileId, nameof(profileId));

        var skills = await _dbContext.ProfileSkills
            .Include(ps => ps.Skill)
            .Where(ps => ps.ProfileId == profileId)
            .Select(ps => ps.Skill.Name)
            .ToListAsync(cancellationToken);

        return skills;
    }
}
