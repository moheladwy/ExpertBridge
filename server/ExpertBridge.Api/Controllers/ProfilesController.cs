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

    [Route("/api/v2/{controller}/onboard")]
    [HttpPost]
    public async Task<ProfileResponse> OnboardUserV2(
        [FromBody] OnboardUserRequestV2 request,
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

    [HttpPut]
    public async Task<ProfileResponse> UpdateProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var user = await _authHelper.GetCurrentUserAsync();
        if (user is null) throw new UnauthorizedAccessException("The user is not authorized.");

        var profile = user.Profile;

        if (!string.IsNullOrEmpty(request.Username)) {
            if (await _dbContext.Profiles.AsNoTracking().AnyAsync(p =>
                    p.Username == request.Username, cancellationToken))
            {
                throw new ProfileUserNameAlreadyExistsException($"Username '{user.Profile.Username}' already exists.");
            }
            if (request.Username != user.Profile.Username)
                profile.Username = request.Username;
        }
        if (!string.IsNullOrEmpty(request.FirstName))
            profile.FirstName = request.FirstName;
        if (!string.IsNullOrEmpty(request.LastName))
            profile.LastName = request.LastName;
        profile.PhoneNumber = request.PhoneNumber;
        profile.Bio = request.Bio;
        profile.JobTitle = request.JobTitle;


        await _dbContext.SaveChangesAsync(cancellationToken);

        var profileResponse = await _dbContext.Profiles
            .FullyPopulatedProfileQuery(p => p.UserId == user.Id)
            .SelectProfileResponseFromProfile()
            .FirstOrDefaultAsync(cancellationToken);

        if (profileResponse == null)
            throw new ProfileNotFoundException($"User[{user.Id}] Profile was not found");

        return profileResponse;
    }
}

