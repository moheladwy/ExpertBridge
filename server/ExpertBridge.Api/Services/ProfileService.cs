using System.Globalization;
using ExpertBridge.Application.DataGenerator;
using ExpertBridge.Application.Helpers;
using ExpertBridge.Contract.Messages;
using ExpertBridge.Contract.Queries;
using ExpertBridge.Contract.Requests.OnboardUser;
using ExpertBridge.Contract.Requests.UpdateProfileRequest;
using ExpertBridge.Contract.Requests.UpdateProfileSkills;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkills;
using ExpertBridge.Core.Entities.ManyToManyRelationships.UserInterests;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Skills;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Data.DatabaseContexts;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace ExpertBridge.Api.Services;

/// <summary>
///     Provides profile management including updates, skill management, and AI-powered recommendations.
/// </summary>
public class ProfileService
{
    private readonly AuthorizationHelper _authHelper;
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly ILogger<ProfileService> _logger;
    private readonly IValidator<OnboardUserRequest> _onboardUserRequestValidator;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IValidator<UpdateProfileRequest> _updateProfileRequestValidator;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProfileService" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for profile and skill operations.</param>
    /// <param name="logger">The logger for diagnostic information.</param>
    /// <param name="updateProfileRequestValidator">The validator for profile update requests.</param>
    /// <param name="onboardUserRequestValidator">The validator for onboard user requests.</param>
    /// <param name="publishEndpoint">The message bus publish endpoint.</param>
    /// <param name="authHelper">The authorization helper for user context.</param>
    public ProfileService(
        ExpertBridgeDbContext dbContext,
        ILogger<ProfileService> logger,
        IValidator<UpdateProfileRequest> updateProfileRequestValidator,
        IValidator<OnboardUserRequest> onboardUserRequestValidator,
        IPublishEndpoint publishEndpoint,
        AuthorizationHelper authHelper)
    {
        _dbContext = dbContext;
        _logger = logger;
        _updateProfileRequestValidator = updateProfileRequestValidator;
        _onboardUserRequestValidator = onboardUserRequestValidator;
        _publishEndpoint = publishEndpoint;
        _authHelper = authHelper;
    }

    /// <summary>
    ///     Retrieves profiles similar to the user based on AI vector embeddings.
    /// </summary>
    /// <param name="userProfile">The profile to find similar profiles for. Can be null for anonymous discovery.</param>
    /// <param name="limit">The maximum number of similar profiles to return.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A list of similar <see cref="ProfileResponse" /> objects.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
    /// <remarks>Uses cosine distance between UserInterestEmbedding vectors. Generates random embedding for anonymous users.</remarks>
    public async Task<List<ProfileResponse>> GetSimilarProfilesAsync(
        Profile? userProfile,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var userEmbedding = userProfile?.UserInterestEmbedding ?? Generator.GenerateRandomVector(1024);

        var query = _dbContext.Profiles
            .AsNoTracking()
            .FullyPopulatedProfileQuery()
            .AsQueryable();

        if (userProfile != null)
        {
            query = query.Where(p => p.Id != userProfile.Id);
        }

        var suggested = await query
            .Where(p => p.UserInterestEmbedding != null)
            .Take(limit)
            .OrderBy(p => p.UserInterestEmbedding.CosineDistance(userEmbedding))
            .SelectProfileResponseFromProfile()
            .ToListAsync(cancellationToken);

        return suggested;
    }

    /// <summary>
    ///     Retrieves profiles with the highest reputation scores, excluding the current user.
    /// </summary>
    /// <param name="userProfile">The current user's profile to exclude from results. Can be null for anonymous browsing.</param>
    /// <param name="limit">The maximum number of top profiles to return.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A list of top <see cref="ProfileResponse" /> objects ordered by reputation score descending.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
    /// <remarks>Reputation is derived from upvotes, completed jobs, ratings, and community contributions.</remarks>
    public async Task<List<ProfileResponse>> GetTopReputationProfilesAsync(
        Profile? userProfile,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Profiles
            .FullyPopulatedProfileQuery()
            .AsQueryable();

        if (userProfile != null)
        {
            query = query.Where(p => p.Id != userProfile.Id);
        }

        var topProfiles = await query
            .Take(limit)
            .SelectProfileResponseFromProfile()
            .ToListAsync(cancellationToken);

        return topProfiles.OrderByDescending(p => p.Reputation).ToList();
    }

    /// <summary>
    ///     Retrieves the current user's profile.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The <see cref="ProfileResponse" />for the current authenticated user.</returns>
    /// <exception cref="UnauthorizedGetMyProfileException">Thrown when the profile cannot be found.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
    public async Task<ProfileResponse> GetCurrentProfileResponseAsync(CancellationToken cancellationToken = default)
    {
        var user = await _authHelper.GetCurrentUserAsync();
        if (user == null)
        {
            _logger.LogWarning("Unauthorized access attempt to GetCurrentProfileResponseAsync");
            throw new UnauthorizedGetMyProfileException();
        }

        _logger.LogInformation("Retrieving profile for user ID: {UserId}", user.Id);

        return await GetProfileResponseFromProfileAsync(user.Profile, cancellationToken);
    }

    /// <summary>
    ///     Retrieves a profile by its Id.
    /// </summary>
    /// <param name="profileId">The Id of the profile to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The <see cref="ProfileResponse" /> for the specified profile ID.</returns>
    /// <exception cref="ArgumentException">Thrown when profileId is null or empty.</exception>
    /// <exception cref="ProfileNotFoundException">Thrown when the profile cannot be found.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
    public async Task<ProfileResponse> GetProfileResponseByIdAsync(
        string profileId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(profileId);

        _logger.LogInformation("Retrieving profile with ID: {ProfileId}", profileId);

        var profile = await _dbContext.Profiles
            .AsNoTracking()
            .Include(p => p.User)
            .Where(p => p.Id == profileId)
            .FirstOrDefaultAsync(cancellationToken);

        if (profile == null)
        {
            _logger.LogWarning("Profile not found with ID: {ProfileId}", profileId);
            throw new ProfileNotFoundException($"Profile with id={profileId} was not found");
        }

        return await GetProfileResponseFromProfileAsync(profile, cancellationToken);
    }

    /// <summary>
    ///     Converts a <see cref="Profile" /> entity to a <see cref="ProfileResponse" /> object asynchronously.
    /// </summary>
    /// <param name="profile">The profile entity to be converted.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the <see cref="ProfileResponse" />
    ///     constructed from the profile.
    /// </returns>
    private async Task<ProfileResponse> GetProfileResponseFromProfileAsync(
        Profile profile,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(profile);
        _logger.LogInformation("Retrieving profile with ID: {ProfileId}", profile.Id);

        var skills = await GetProfileSkillsByProfileIdAsync(profile.Id, cancellationToken);

        var comments = await _dbContext.CommentVotes
            .AsNoTracking()
            .Include(cv => cv.Comment)
            .Where(cv => cv.Comment.AuthorId == profile.Id)
            .Select(cv => cv.IsUpvote)
            .ToListAsync(cancellationToken);
        _logger.LogInformation("Retrieved {CommentCount} comments for profile ID: {ProfileId}", comments.Count,
            profile.Id);

        var upvotes = comments.Count(c => c);
        _logger.LogInformation("Retrieved {UpvoteCount} upvotes for profile ID: {ProfileId}", upvotes, profile.Id);

        var downvotes = comments.Count - upvotes;
        _logger.LogInformation("Retrieved {DownvoteCount} downvotes for profile ID: {ProfileId}", downvotes,
            profile.Id);

        var response = new ProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            CreatedAt = profile.CreatedAt.Value,
            Email = profile.Email,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            IsBanned = profile.IsBanned,
            JobTitle = profile.JobTitle,
            Bio = profile.Bio,
            PhoneNumber = profile.PhoneNumber,
            ProfilePictureUrl = profile.ProfilePictureUrl,
            Rating = profile.Rating,
            RatingCount = profile.RatingCount,
            Username = profile.Username,
            IsOnboarded = profile.User.IsOnboarded,
            Skills = skills,
            CommentsUpvotes = upvotes,
            CommentsDownvotes = downvotes,
            Reputation = upvotes - downvotes
        };

        _logger.LogInformation("Successfully retrieved profile with ID: {ProfileId}", profile.Id);
        return response;
    }

    /// <summary>
    ///     Onboards a user by setting their interests and marking them as onboarded.
    /// </summary>
    /// <param name="user">The user entity to onboard.</param>
    /// <param name="request">The onboarding request containing the user's interest tags.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The updated <see cref="ProfileResponse" /> after onboarding.</returns>
    /// <exception cref="ArgumentNullException">Thrown when user or request is null.</exception>
    /// <exception cref="ValidationException">Thrown when the request fails validation.</exception>
    /// <exception cref="ProfileNotFoundException">Thrown when the profile cannot be found after onboarding.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
    public async Task<ProfileResponse> OnboardUserAsync(
        User user,
        OnboardUserRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation("Starting onboarding process for user ID: {UserId}", user.Id);

        // Validate the request using the validator from FluentValidation.
        var validationResult = await _onboardUserRequestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Onboarding validation failed for user ID: {UserId}", user.Id);
            throw new ValidationException(validationResult.Errors);
        }

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

        _logger.LogInformation("User onboarded successfully. User ID: {UserId}, New tags to process: {TagCount}",
            user.Id, newTagsToBeProcessed.Count);

        await _publishEndpoint.Publish(
            new UserInterestsProsessingMessage
            {
                UserProfileId = user.Profile.Id, InterestsTags = newTagsToBeProcessed
            }, cancellationToken);

        var response = await GetProfileResponseByIdAsync(user.Profile.Id, cancellationToken);

        _logger.LogInformation("Successfully completed onboarding for user ID: {UserId}", user.Id);
        return response;
    }

    /// <summary>
    ///     Checks if a username is available for use.
    /// </summary>
    /// <param name="username">The username to check availability for.</param>
    /// <param name="currentUserProfile">The current user's profile to exclude from the check.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>True if the username is available, false otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when username is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when currentUserProfile is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
    public async Task<bool> IsUsernameAvailableAsync(
        string username,
        Profile currentUserProfile,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentNullException.ThrowIfNull(currentUserProfile);

        _logger.LogInformation("Checking username availability: {Username} for user ID: {UserId}",
            username, currentUserProfile.UserId);

        // Check if the username is the same as the current user's username
        // if so, return false since it's already taken by the current user.
        if (currentUserProfile.Username == username)
        {
            _logger.LogInformation("Username {Username} is the current user's username", username);
            return false;
        }

        var isAvailable = !await _dbContext.Profiles
            .AsNoTracking()
            .AnyAsync(
                p => p.Username == username,
                cancellationToken
            );

        _logger.LogInformation("Username {Username} availability: {IsAvailable}", username, isAvailable);
        return isAvailable;
    }

    /// <summary>
    ///     Retrieves the skills for a specific profile.
    /// </summary>
    /// <param name="profileId">The ID of the profile whose skills to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A list of skill names associated with the profile.</returns>
    /// <exception cref="ArgumentException">Thrown when profileId is null or empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
    public async Task<List<string>> GetProfileSkillsByProfileIdAsync(
        string profileId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(profileId);

        _logger.LogInformation("Retrieving skills for profile ID: {ProfileId}", profileId);

        var skills = await _dbContext.ProfileSkills
            .Include(ps => ps.Skill)
            .Where(ps => ps.ProfileId == profileId)
            .Select(ps => ps.Skill.Name)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {SkillCount} skills for profile ID: {ProfileId}",
            skills.Count, profileId);

        return skills;
    }

    /// <summary>
    ///     Updates a user's profile information including personal details and skills.
    /// </summary>
    /// <param name="user">The user entity whose profile is being updated.</param>
    /// <param name="request">The profile update request containing new field values.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the updated <see cref="ProfileResponse" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when user or request is null.</exception>
    /// <exception cref="ValidationException">
    ///     Thrown when the request fails validation (invalid format, missing required fields).
    /// </exception>
    /// <exception cref="ProfileUserNameAlreadyExistsException">
    ///     Thrown when the requested username is already taken by another user.
    /// </exception>
    /// <exception cref="ProfilePhoneNumberAlreadyExistsException">
    ///     Thrown when the requested phone number is already registered to another user.
    /// </exception>
    /// <exception cref="ProfileNotFoundException">
    ///     Thrown when the profile cannot be found after update (data consistency issue).
    /// </exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
    /// <remarks>
    ///     Profile is re-fetched with all navigation properties for a complete response.
    /// </remarks>
    public async Task<ProfileResponse> UpdateProfileAsync(
        User user,
        UpdateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(request);
        // Validate the request using the validator from FluentValidation.
        var validationResult = await _updateProfileRequestValidator
            .ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var profile = user.Profile;

        if (!string.IsNullOrEmpty(request.Username) &&
            request.Username != user.Profile.Username)
        {
            var isUsernameExists = await _dbContext.Profiles
                .AsNoTracking()
                .AnyAsync(p => p.Username == request.Username && p.Username != user.Profile.Username,
                    cancellationToken);
            if (isUsernameExists)
            {
                throw new ProfileUserNameAlreadyExistsException($"Username '{user.Profile.Username}' already exists.");
            }

            profile.Username = request.Username;
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber) &&
            request.PhoneNumber != user.Profile.PhoneNumber)
        {
            var isPhoneNumberExisting = await _dbContext.Profiles
                .AsNoTracking()
                .AnyAsync(p =>
                        p.PhoneNumber == request.PhoneNumber && p.PhoneNumber != user.Profile.PhoneNumber,
                    cancellationToken);
            if (isPhoneNumberExisting)
            {
                throw new ProfilePhoneNumberAlreadyExistsException(
                    $"Phone number '{request.PhoneNumber}' already exists.");
            }

            profile.PhoneNumber = request.PhoneNumber;
        }

        if (!string.IsNullOrEmpty(request.FirstName) && request.FirstName != user.Profile.FirstName)
        {
            profile.FirstName = request.FirstName;
        }

        if (!string.IsNullOrEmpty(request.LastName) && request.LastName != user.Profile.LastName)
        {
            profile.LastName = request.LastName;
        }

        if (!string.IsNullOrEmpty(request.Bio) && request.Bio != user.Profile.Bio)
        {
            profile.Bio = request.Bio;
        }

        if (!string.IsNullOrEmpty(request.JobTitle) && request.JobTitle != user.Profile.JobTitle)
        {
            profile.JobTitle = request.JobTitle;
        }

        await UpdateProfileSkillsAsync(
            profile,
            new UpdateProfileSkillsRequest { Skills = request.Skills },
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetProfileResponseByIdAsync(profile.Id, cancellationToken);
    }

    /// <summary>
    ///     Internal method that updates a profile's skills with normalization and deduplication.
    /// </summary>
    /// <param name="profile">The profile entity to update skills for.</param>
    /// <param name="request">The skills update request containing the new skill list.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when profile or request is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
    /// <remarks>
    ///     Normalizes skill names (lowercase, trimmed), removes duplicates, creates new skills if needed, and maintains
    ///     ProfileSkill relationships.
    /// </remarks>
    private async Task UpdateProfileSkillsAsync(
        Profile profile,
        UpdateProfileSkillsRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(request);

        // Normalize skills: trim whitespace, convert to lowercase, and remove duplicates
        request.Skills = request.Skills
            .Where(skill => !string.IsNullOrWhiteSpace(skill))
            .Select(skill => skill.Trim().ToLower(CultureInfo.InvariantCulture))
            .Distinct()
            .ToList();

        // get the exiting skills from the database.
        var existingSkills = await _dbContext.Skills
            .AsNoTracking()
            .Where(skill => request.Skills.Contains(skill.Name))
            .ToListAsync(cancellationToken);

        var skillsToBeAdded = request.Skills
            .Where(skill => existingSkills
                .All(existingSkill => !existingSkill.Name.Equals(skill, StringComparison.OrdinalIgnoreCase)))
            .Select(skillName => new Skill { Name = skillName })
            .ToList();

        await _dbContext.Skills.AddRangeAsync(skillsToBeAdded, cancellationToken);

        var allSkills = existingSkills.Concat(skillsToBeAdded).ToList();

        var existingProfileSkills = await _dbContext.ProfileSkills
            .Include(ps => ps.Skill)
            .Where(ps => ps.ProfileId == profile.Id)
            .ToListAsync(cancellationToken);

        var profileSkillsToBeAdded = request.Skills
            .Where(skill => existingProfileSkills
                .All(existingSkill => !existingSkill.Skill.Name
                    .Equals(skill, StringComparison.OrdinalIgnoreCase)))
            .Select(skillName => new ProfileSkill
            {
                ProfileId = profile.Id,
                SkillId = allSkills
                    .First(s => s.Name.Equals(skillName, StringComparison.OrdinalIgnoreCase))
                    .Id
            })
            .ToList();

        var profileSkillsToBeRemoved = existingProfileSkills
            .Where(ps => !request.Skills.Contains(ps.Skill.Name))
            .ToList();

        _dbContext.ProfileSkills.RemoveRange(profileSkillsToBeRemoved);

        await _dbContext.ProfileSkills.AddRangeAsync(profileSkillsToBeAdded, cancellationToken);
    }
}
