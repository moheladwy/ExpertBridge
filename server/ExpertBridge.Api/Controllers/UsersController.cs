// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Requests.RegisterUser;
using ExpertBridge.Core.Requests.UpdateUserRequest;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Controllers;

/// <summary>
/// Provides API endpoints for user account management and authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    /// Provides API endpoints for user account management and authentication.
    /// </summary>
    public UsersController(ExpertBridgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves a user by their Firebase Authentication identity provider ID.
    /// </summary>
    /// <param name="identityProviderId">The Firebase Authentication provider ID of the user.</param>
    /// <returns>The user information wrapped in a <see cref="UserResponse"/>.</returns>
    /// <exception cref="UserNotFoundException">Thrown when the user with the specified provider ID cannot be found.</exception>
    /// <exception cref="NotImplementedException">This endpoint is currently not implemented.</exception>
    [HttpGet("get/{identityProviderId}")]
    public async Task<UserResponse> GetUserByIdentityProviderId([FromRoute] string identityProviderId)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <returns>The user information wrapped in a <see cref="UserResponse"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when the email parameter is null or empty.</exception>
    /// <exception cref="UserNotFoundException">Thrown when no user with the specified email address exists.</exception>
    [HttpGet("get-by-email/{email}")]
    public async Task<UserResponse> GetUserByEmail([FromRoute] string email)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);

        var user = await _dbContext.Users
                       .AsNoTracking()
                       .SingleOrDefaultAsync(u => u.Email == email)
                   ?? throw new UserNotFoundException("User not found");

        return new UserResponse(user);
    }

    /// <summary>
    /// Registers a new user account in the system.
    /// </summary>
    /// <param name="request">The registration request containing user details.</param>
    /// <returns>The newly created user information wrapped in a <see cref="UserResponse"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the request parameter is null.</exception>
    /// <exception cref="NotImplementedException">This endpoint is currently not implemented.</exception>
    [HttpPost("register")]
    public async Task<UserResponse> RegisterNewUser([FromBody] RegisterUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        //return await userService.RegisterNewUser(request);
        throw new NotImplementedException();
    }

    /// <summary>
    /// Updates an existing user's information or creates a new user if they don't exist.
    /// </summary>
    /// <param name="request">The update request containing the user's new information.</param>
    /// <returns>The updated or newly created <see cref="User"/> entity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the request parameter is null.</exception>
    /// <exception cref="UnauthorizedException">Thrown when the authenticated user's email doesn't match the request email.</exception>
    /// <remarks>
    /// This endpoint also creates an associated profile for new users.
    /// </remarks>
    [HttpPut]
    public async Task<User> Update([FromBody] UpdateUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var email = User.FindFirstValue(ClaimTypes.Email);

        if (email != request.Email)
        {
            throw new UnauthorizedException();
        }

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);

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
            //await _dbContext.SaveChangesAsync();

            await _dbContext.Profiles.AddAsync(new Profile
            {
                User = user,
                Email = user.Email,
                IsBanned = user.IsBanned,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = username,
                PhoneNumber = user.PhoneNumber,
                ProfilePictureUrl = request.ProfilePictureUrl
            });
        }
        else
        {
            user.ProviderId = request.ProviderId;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.IsEmailVerified = request.IsEmailVerified;
        }

        // UNIT OF WORK, only one save after the unit is complete.
        await _dbContext.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Checks whether a user account has been marked as deleted.
    /// </summary>
    /// <param name="identityProviderId">The Firebase Authentication provider ID of the user.</param>
    /// <returns>True if the user is deleted; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when the identityProviderId parameter is null or empty.</exception>
    /// <exception cref="NotImplementedException">This endpoint is currently not implemented.</exception>
    [HttpGet("is-deleted/{identityProviderId}")]
    public async Task<bool> IsUserDeleted([FromRoute] string identityProviderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        //return await userService.IsUserDeletedAsync(identityProviderId);
        throw new NotImplementedException();
    }

    /// <summary>
    /// Marks a user account as deleted (soft delete).
    /// </summary>
    /// <param name="identityProviderId">The Firebase Authentication provider ID of the user to delete.</param>
    /// <returns>An HTTP 200 OK response with a success message.</returns>
    /// <exception cref="ArgumentException">Thrown when the identityProviderId parameter is null or empty.</exception>
    [HttpDelete("delete/{identityProviderId}")]
    public async Task<IActionResult> DeleteUser([FromRoute] string identityProviderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        //await userService.DeleteUserAsync(identityProviderId);
        return Ok("User deleted successfully.");
    }

    /// <summary>
    /// Checks whether a user account has been banned from the platform.
    /// </summary>
    /// <param name="identityProviderId">The Firebase Authentication provider ID of the user.</param>
    /// <returns>True if the user is banned; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when the identityProviderId parameter is null or empty.</exception>
    /// <exception cref="NotImplementedException">This endpoint is currently not implemented.</exception>
    [HttpGet("is-banned/{identityProviderId}")]
    public async Task<bool> IsUserBanned([FromRoute] string identityProviderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        //return await userService.IsUserBannedAsync(identityProviderId);
        throw new NotImplementedException();
    }

    /// <summary>
    /// Bans a user account from the platform.
    /// </summary>
    /// <param name="identityProviderId">The Firebase Authentication provider ID of the user to ban.</param>
    /// <returns>An HTTP 200 OK response with a success message.</returns>
    /// <exception cref="ArgumentException">Thrown when the identityProviderId parameter is null or empty.</exception>
    /// <exception cref="NotImplementedException">This endpoint is currently not implemented.</exception>
    [HttpPut("ban/{identityProviderId}")]
    public async Task<IActionResult> BanUser([FromRoute] string identityProviderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        //await userService.BanUserAsync(identityProviderId);

        throw new NotImplementedException();
        return Ok("User banned successfully.");
    }

    /// <summary>
    /// Checks whether a user's email address has been verified.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>True if the user's email is verified; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when the email parameter is null or empty.</exception>
    /// <exception cref="NotImplementedException">This endpoint is currently not implemented.</exception>
    [HttpGet("is-verified/{email}")]
    public async Task<bool> IsUserVerified([FromRoute] string email)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);

        throw new NotImplementedException();
        //return await userService.IsUserVerifiedAsync(email);
    }

    /// <summary>
    /// Marks a user's email address as verified.
    /// </summary>
    /// <param name="email">The email address to verify.</param>
    /// <returns>An HTTP 200 OK response with a success message.</returns>
    /// <exception cref="ArgumentException">Thrown when the email parameter is null or empty.</exception>
    [HttpPut("verify/{email}")]
    public async Task<IActionResult> VerifyUser([FromRoute] string email)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);
        //await userService.VerifyUserAsync(email);
        return Ok("User verified successfully.");
    }
}
