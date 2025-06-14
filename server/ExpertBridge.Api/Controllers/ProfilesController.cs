// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Api.Settings;
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
    private readonly ChannelWriter<UserInterestsProsessingMessage> _channelWriter;
    private readonly IValidator<UpdateProfileRequest> _updateProfileRequestValidator;

    public ProfilesController(
        ExpertBridgeDbContext dbContext,
        AuthorizationHelper authHelper,
        Channel<UserInterestsProsessingMessage> channel,
        IValidator<UpdateProfileRequest> updateProfileRequestValidator)
    {
        _dbContext = dbContext;
        _authHelper = authHelper;
        _channelWriter = channel.Writer;
        _updateProfileRequestValidator = updateProfileRequestValidator;
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

    [Route("/api/v2/Profiles/onboard")]
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

    [Authorize]
    [HttpPut]
    public async Task<ProfileResponse> UpdateProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _authHelper.GetCurrentUserAsync();
        if (user is null)
            throw new UnauthorizedAccessException("The user is not authorized.");

        ArgumentNullException.ThrowIfNull(request, nameof(request));

        // Validate the request using the validator from FluentValidation.
        var validationResult = await _updateProfileRequestValidator
            .ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var profile = user.Profile;

        if (!string.IsNullOrEmpty(request.Username) &&
                request.Username != user.Profile.Username)
        {
            var isUsernameExisting = await _dbContext.Profiles
                .AsNoTracking()
                .AnyAsync(p => p.Username == request.Username,
                        cancellationToken);
            if (isUsernameExisting)
                throw new ProfileUserNameAlreadyExistsException(
                        $"Username '{user.Profile.Username}' already exists.");
            profile.Username = request.Username;
        }
        if (!string.IsNullOrEmpty(request.PhoneNumber) &&
                request.PhoneNumber != user.Profile.PhoneNumber)
        {
            var isPhoneNumberExisting = await _dbContext.Profiles
                .AsNoTracking()
                .AnyAsync(
                        p => p.PhoneNumber == request.PhoneNumber,
                        cancellationToken);
            if (isPhoneNumberExisting)
                throw new ProfilePhoneNumberAlreadyExistsException(
                        $"Phone number '{request.PhoneNumber}' already exists.");
            profile.PhoneNumber = request.PhoneNumber;
        }
        if (!string.IsNullOrEmpty(request.FirstName) && request.FirstName != user.Profile.FirstName)
            profile.FirstName = request.FirstName;
        if (!string.IsNullOrEmpty(request.LastName) && request.LastName != user.Profile.LastName)
            profile.LastName = request.LastName;
        if (!string.IsNullOrEmpty(request.Bio) && request.Bio != user.Profile.Bio)
            profile.Bio = request.Bio;
        if (!string.IsNullOrEmpty(request.JobTitle) && request.JobTitle != user.Profile.JobTitle)
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
}
