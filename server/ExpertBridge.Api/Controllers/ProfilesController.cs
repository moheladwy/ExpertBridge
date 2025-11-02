using ExpertBridge.Application.DomainServices;
using ExpertBridge.Application.Helpers;
using ExpertBridge.Application.Settings;
using ExpertBridge.Contract.Requests.OnboardUser;
using ExpertBridge.Contract.Requests.UpdateProfileRequest;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     Provides API endpoints for profile management, including retrieval, updates, onboarding, and skill management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly AuthorizationHelper _authHelper;
    private readonly ProfileService _profileService;
    private readonly UserService _userService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProfilesController" /> class.
    /// </summary>
    /// <param name="authHelper">Helper for user authorization operations.</param>
    /// <param name="userService">Service for user-related operations.</param>
    /// <param name="profileService">Service for profile management operations.</param>
    public ProfilesController(
        AuthorizationHelper authHelper,
        UserService userService,
        ProfileService profileService)
    {
        _authHelper = authHelper;
        _userService = userService;
        _profileService = profileService;
    }

    /// <summary>
    ///     Retrieves the profile of the currently authenticated user.
    /// </summary>
    /// <returns>The <see cref="ProfileResponse" /> for the authenticated user.</returns>
    /// <response code="200">Returns the user's profile.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If the user's profile is not found.</response>
    /// <exception cref="UnauthorizedGetMyProfileException">Thrown when the user is not authenticated.</exception>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ProfileResponse> GetProfile()
    {
        var user = await _authHelper.GetCurrentUserAsync();
        if (user == null)
        {
            throw new UnauthorizedGetMyProfileException();
        }

        return await _profileService.GetProfileByUserIdAsync(user.Id);
    }

    /// <summary>
    ///     Retrieves a profile by its ID. This endpoint is cached for performance.
    /// </summary>
    /// <param name="id">The ID of the profile to retrieve.</param>
    /// <returns>The <see cref="ProfileResponse" /> for the specified profile ID.</returns>
    /// <response code="200">Returns the profile with the specified ID.</response>
    /// <response code="404">If the profile is not found.</response>
    /// <exception cref="ProfileNotFoundException">Thrown when the profile with the specified ID is not found.</exception>
    [AllowAnonymous]
    [HttpGet("{id}")]
    [ResponseCache(CacheProfileName = CacheProfiles.Default)]
    public async Task<ProfileResponse> GetProfile(string id)
    {
        return await _profileService.GetProfileByIdAsync(id);
    }

    /// <summary>
    ///     Onboards a user by setting their interests and marking them as onboarded.
    /// </summary>
    /// <param name="request">The onboarding request containing the user's interest tags.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The updated <see cref="ProfileResponse" /> after onboarding.</returns>
    /// <response code="200">Returns the updated profile after successful onboarding.</response>
    /// <response code="400">If the request validation fails.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If the user's profile is not found.</response>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated.</exception>
    [HttpPost("onboard")]
    public async Task<ProfileResponse> OnboardUser(
        [FromBody] OnboardUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _authHelper.GetCurrentUserAsync();
        if (user is null)
        {
            throw new UnauthorizedAccessException("The user is not authorized.");
        }

        return await _profileService.OnboardUserAsync(user, request, cancellationToken);
    }

    /// <summary>
    ///     Updates the authenticated user's profile information including personal details and skills.
    /// </summary>
    /// <param name="request">The profile update request containing new field values.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The updated <see cref="ProfileResponse" />.</returns>
    /// <response code="200">Returns the updated profile.</response>
    /// <response code="400">If the request validation fails or username/phone number already exists.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If the user's profile is not found.</response>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated.</exception>
    [Authorize]
    [HttpPut]
    public async Task<ProfileResponse> UpdateProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userService.GetCurrentUserPopulatedModelAsync();
        if (user is null)
        {
            throw new UnauthorizedAccessException("The user is not authorized.");
        }

        var profileResponse = await _profileService.UpdateProfileAsync(
            user,
            request,
            cancellationToken);

        return profileResponse;
    }

    /// <summary>
    ///     Checks if a username is available for use by the authenticated user.
    /// </summary>
    /// <param name="username">The username to check availability for.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>True if the username is available, false if it's already taken or is the current user's username.</returns>
    /// <response code="200">Returns true if the username is available, false otherwise.</response>
    /// <response code="400">If the username is null, empty, or whitespace.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated.</exception>
    [Authorize]
    [HttpGet("is-username-available/{username}")]
    public async Task<bool> IsUsernameAvailable(
        [FromRoute] string username,
        CancellationToken cancellationToken = default)
    {
        var user = await _authHelper.GetCurrentUserAsync();
        if (user is null)
        {
            throw new UnauthorizedAccessException("The user is not authorized.");
        }

        return await _profileService.IsUsernameAvailableAsync(username, user.Profile, cancellationToken);
    }

    /// <summary>
    ///     Retrieves profiles similar to the current user based on AI vector embeddings.
    ///     For anonymous users, returns random suggestions.
    /// </summary>
    /// <param name="limit">The maximum number of profiles to return. Defaults to 5 if not specified.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A list of suggested <see cref="ProfileResponse" /> objects.</returns>
    /// <response code="200">Returns a list of suggested profiles.</response>
    /// <remarks>
    ///     Uses cosine distance between UserInterestEmbedding vectors for authenticated users.
    ///     Generates random embedding for anonymous users to provide diverse suggestions.
    /// </remarks>
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

    /// <summary>
    ///     Retrieves profiles with the highest reputation scores.
    ///     Excludes the current user's profile if authenticated.
    /// </summary>
    /// <param name="limit">The maximum number of profiles to return. Defaults to 5 if not specified.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A list of top <see cref="ProfileResponse" /> objects ordered by reputation score descending.</returns>
    /// <response code="200">Returns a list of top reputation profiles.</response>
    /// <remarks>
    ///     Reputation is derived from upvotes, completed jobs, ratings, and community contributions.
    ///     The current user's profile is excluded from results when authenticated.
    /// </remarks>
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

    /// <summary>
    ///     Retrieves the skills associated with the authenticated user's profile.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A list of skill names associated with the user's profile.</returns>
    /// <response code="200">Returns a list of skill names.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated.</exception>
    [Authorize]
    [HttpGet("skills")]
    public async Task<List<string>> GetCurrentUserSkills(CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetCurrentUserPopulatedModelAsync();
        if (user is null)
        {
            throw new UnauthorizedAccessException("The user is not authorized.");
        }

        return await _profileService.GetProfileSkillsAsync(user.Profile.Id, cancellationToken);
    }

    /// <summary>
    ///     Retrieves the skills associated with a specific profile by profile ID.
    /// </summary>
    /// <param name="profileId">The ID of the profile whose skills to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A list of skill names associated with the specified profile.</returns>
    /// <response code="200">Returns a list of skill names.</response>
    /// <response code="400">If the profileId is null or empty.</response>
    [AllowAnonymous]
    [HttpGet("{profileId}/skills")]
    public async Task<List<string>> GetProfileSkills(
        [FromRoute] string profileId,
        CancellationToken cancellationToken = default)
    {
        return await _profileService.GetProfileSkillsAsync(profileId, cancellationToken);
    }
}
