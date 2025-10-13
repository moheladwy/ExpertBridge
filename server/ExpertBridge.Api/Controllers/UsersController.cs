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

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController(
    //IUsersService userService,
    ExpertBridgeDbContext _dbContext
) : ControllerBase
{
    [HttpGet("get/{identityProviderId}")]
    public async Task<UserResponse> GetUserByIdentityProviderId([FromRoute] string identityProviderId) => throw
        //ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        //return await userService.GetUserByIdentityProviderId(identityProviderId);
        new NotImplementedException();


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

    [HttpPost("register")]
    public async Task<UserResponse> RegisterNewUser([FromBody] RegisterUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        //return await userService.RegisterNewUser(request);
        throw new NotImplementedException();
    }

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

    [HttpGet("is-deleted/{identityProviderId}")]
    public async Task<bool> IsUserDeleted([FromRoute] string identityProviderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        //return await userService.IsUserDeletedAsync(identityProviderId);
        throw new NotImplementedException();
    }

    [HttpDelete("delete/{identityProviderId}")]
    public async Task<IActionResult> DeleteUser([FromRoute] string identityProviderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        //await userService.DeleteUserAsync(identityProviderId);
        return Ok("User deleted successfully.");
    }

    [HttpGet("is-banned/{identityProviderId}")]
    public async Task<bool> IsUserBanned([FromRoute] string identityProviderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        //return await userService.IsUserBannedAsync(identityProviderId);
        throw new NotImplementedException();
    }

    [HttpPut("ban/{identityProviderId}")]
    public async Task<IActionResult> BanUser([FromRoute] string identityProviderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        //await userService.BanUserAsync(identityProviderId);

        throw new NotImplementedException();
        return Ok("User banned successfully.");
    }

    [HttpGet("is-verified/{email}")]
    public async Task<bool> IsUserVerified([FromRoute] string email)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);

        throw new NotImplementedException();
        //return await userService.IsUserVerifiedAsync(email);
    }

    [HttpPut("verify/{email}")]
    public async Task<IActionResult> VerifyUser([FromRoute] string email)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);
        //await userService.VerifyUserAsync(email);
        return Ok("User verified successfully.");
    }
}
