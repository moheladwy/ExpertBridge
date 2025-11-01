// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using ExpertBridge.Contract.Messages;
using ExpertBridge.Contract.Requests.UpdateUserRequest;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Data.DatabaseContexts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     Provides API endpoints for user account management and authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    /// <summary>
    ///     Represents the database context for accessing and interacting with the ExpertBridge database.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Represents the MassTransit publish endpoint used for publishing events to the message bus.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Provides API endpoints for user account management and authentication.
    /// </summary>
    public UsersController(
        ExpertBridgeDbContext dbContext,
        IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <returns>The user information wrapped in a <see cref="UserResponse" />.</returns>
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
    ///     Updates an existing user's information or creates a new user if they don't exist.
    /// </summary>
    /// <param name="request">The update request containing the user's new information.</param>
    /// <returns>The updated or newly created <see cref="User" /> entity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the request parameter is null.</exception>
    /// <exception cref="UnauthorizedException">Thrown when the authenticated user's email doesn't match the request email.</exception>
    /// <remarks>
    ///     This endpoint also creates an associated profile for new users.
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
            .Include(u => u.Profile)
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

        // UNIT OF WORK, only one save after the unit is complete.
        await _dbContext.SaveChangesAsync();

        return user;
    }
}
