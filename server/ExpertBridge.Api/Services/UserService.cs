using ExpertBridge.Application.Helpers;
using ExpertBridge.Contract.Messages;
using ExpertBridge.Contract.Requests.UpdateUserRequest;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Data.DatabaseContexts;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Services;

/// <summary>
///     Provides user-related operations for retrieving the current authenticated user and their profile.
/// </summary>
/// <remarks>
///     This service wraps <see cref="AuthorizationHelper" /> to provide convenient methods for accessing
///     the currently authenticated user's information within the HTTP request context.
///     Authentication is based on Firebase JWT claims where User.Claims["provider_id"] maps to User.ProviderId.
///     The service requires HttpContext and will not work in background workers.
///     Method selection:
///     - Use <see cref="GetCurrentUserPopulatedModelAsync" /> for full User entity with Profile
///     - Use <see cref="GetCurrentUserProfileIdOrEmptyAsync" /> for optional authentication scenarios
///     - Use <see cref="GetCurrentUserProfileOrThrowAsync" /> for endpoints requiring guaranteed authentication
///     - Use <see cref="UpdateUserAsync" /> to create or update user information
/// </remarks>
public sealed class UserService
{
    /// <summary>
    ///     Provides functionality to assist with user authorization and retrieval of current user information.
    /// </summary>
    private readonly AuthorizationHelper _authHelper;

    /// <summary>
    ///     Represents the database context for accessing and interacting with the ExpertBridge database.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Represents the MassTransit publish endpoint used for publishing events to the message bus.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Responsible for validating instances of <see cref="UpdateUserRequest" /> to ensure they meet defined rules and
    ///     constraints.
    /// </summary>
    private readonly IValidator<UpdateUserRequest> _updateUserRequestValidator;

    /// <summary>
    ///     Initializes a new instance of the <see cref="UserService" /> class.
    /// </summary>
    /// <param name="authHelper">
    ///     The authorization helper providing authentication context and user retrieval capabilities.
    /// </param>
    /// <param name="dbContext">
    ///     The database context for accessing the ExpertBridge database.
    /// </param>
    /// <param name="publishEndpoint">
    ///     The MassTransit publish endpoint for event publishing.
    /// </param>
    /// <param name="updateUserRequestValidator">
    ///     The validator for validating UpdateUserRequest instances.
    /// </param>
    public UserService(AuthorizationHelper authHelper,
        ExpertBridgeDbContext dbContext,
        IPublishEndpoint publishEndpoint,
        IValidator<UpdateUserRequest> updateUserRequestValidator)
    {
        _authHelper = authHelper;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
        _updateUserRequestValidator = updateUserRequestValidator;
    }

    /// <summary>
    ///     Retrieves the current authenticated User entity with populated Profile navigation property.
    /// </summary>
    /// <returns>
    ///     The authenticated <see cref="User" /> with Profile, or null if not authenticated.
    /// </returns>
    /// <remarks>
    ///     Use when you need full user data (email, provider ID) in addition to profile information.
    ///     Performs a single database query with eager loading. Never throws exceptions.
    /// </remarks>
    public async Task<User?> GetCurrentUserPopulatedModelAsync()
    {
        return await _authHelper.GetCurrentUserAsync();
    }

    /// <summary>
    ///     Retrieves the Profile ID of the current authenticated user.
    /// </summary>
    /// <returns>
    ///     The Profile ID if authenticated, or <see cref="string.Empty" /> if not authenticated or profile doesn't exist.
    /// </returns>
    /// <remarks>
    ///     Ideal for [AllowAnonymous] endpoints that provide personalized content when authenticated
    ///     but still work for anonymous users. Never throws exceptions.
    /// </remarks>
    public async Task<string> GetCurrentUserProfileIdOrEmptyAsync()
    {
        var user = await _authHelper.GetCurrentUserAsync();
        return user?.Profile?.Id ?? string.Empty;
    }

    /// <summary>
    ///     Updates a user entity in the database based on the provided request data.
    /// </summary>
    /// <param name="request">
    ///     The update user request containing the necessary information to update the user.
    /// </param>
    /// <returns>
    ///     The updated user entity.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     Thrown when the provided request is null.
    /// </exception>
    /// <exception cref="FluentValidation.ValidationException">
    ///     Thrown if the validation of the provided request fails.
    /// </exception>
    public async Task<User> UpdateUserAsync(UpdateUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validationResult = await _updateUserRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var user = await _dbContext.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            var username = $"{request.Email.Split("@")[0]}_{Guid.NewGuid()}";
            user = new User
            {
                ProviderId = request.ProviderId,
                IsEmailVerified = request.IsEmailVerified,
                Email = request.Email,
                FirstName = request.FirstName,
                Username = username,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber
            };

            await _dbContext.Users.AddAsync(user);

            var profile = new Profile
            {
                User = user,
                Email = user.Email,
                IsBanned = user.IsBanned,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = username,
                PhoneNumber = user.PhoneNumber,
                ProfilePictureUrl = request.ProfilePictureUrl
            };
            await _dbContext.Profiles.AddAsync(profile);

            if (string.IsNullOrWhiteSpace(request.ProfilePictureUrl))
            {
                await _publishEndpoint.Publish(new MoveProfileImageFromGoogleToS3Message { ProfileId = profile.Id });
            }
        }
        else
        {
            user.ProviderId = request.ProviderId;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.IsEmailVerified = request.IsEmailVerified;
            if (string.IsNullOrWhiteSpace(user.Profile.ProfilePictureUrl))
            {
                user.Profile.ProfilePictureUrl = request.ProfilePictureUrl;
            }
        }

        await _dbContext.SaveChangesAsync();

        return user;
    }

    /// <summary>
    ///     Retrieves the Profile of the current authenticated user, throwing an exception if not authenticated.
    /// </summary>
    /// <returns>
    ///     The authenticated user's <see cref="Profile" />.
    /// </returns>
    /// <exception cref="UnauthorizedException">
    ///     Thrown when the user is not authenticated or has no associated profile.
    /// </exception>
    /// <exception cref="UserNotFoundException">
    ///     Thrown when the profile is not found in the database (data integrity issue).
    /// </exception>
    /// <remarks>
    ///     Use in [Authorize] endpoints requiring guaranteed authentication (e.g., creating posts, updating profile).
    ///     Validates authentication and profile existence. The returned Profile always belongs to the authenticated user.
    /// </remarks>
    public async Task<Profile> GetCurrentUserProfileOrThrowAsync()
    {
        var user = await _authHelper.GetCurrentUserAsync();

        if (user == null)
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        if (string.IsNullOrEmpty(user.Profile?.Id))
        {
            // This case implies a data integrity issue or incomplete user setup
            // Log this as a server-side issue
            // Serilog.Log.Error("Authenticated user {UserId} has no associated ProfileId.", user.Id);
            throw new UnauthorizedException("User profile identifier is missing.");
        }

        if (user.Profile == null)
        {
            // Serilog.Log.Error("Profile not found in database for ProfileId {ProfileId} associated with User {UserId}.", user.Profile.Id, user.Id);
            throw new UserNotFoundException(
                $"User profile with id={user.Profile.Id} was not found."); // Or a generic Unauthorized
        }

        return user.Profile;
    }
}
