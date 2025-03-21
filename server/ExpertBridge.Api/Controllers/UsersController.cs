// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.Interfaces.Services;
using ExpertBridge.Api.Data.DatabaseContexts;
using ExpertBridge.Api.Requests.RegisterUser;
using ExpertBridge.Api.Requests.UpdateUserRequest;
using ExpertBridge.Api.Responses;
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
    public async Task<UserResponse> GetUserByIdentityProviderId([FromRoute] string identityProviderId)
    {
        //ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        //return await userService.GetUserByIdentityProviderId(identityProviderId);
        throw new NotImplementedException();
    }


    [HttpGet("get-by-email/{email}")]
    public async Task<UserResponse> GetUserByEmail([FromRoute] string email)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);

        //_dbContext.Users.Add(new User
        //{
        //    ProviderId = "asdf",
        //    FirstName = "Hello",
        //    LastName = "Delme",
        //    Email = "y.m.elkilany@gmail.com",
        //    Username = "y.m.elkilany@gmail.com",
        //    PhoneNumber = "01013647953",
        //    IsBanned = false,
        //    IsDeleted = false,
        //    IsEmailVerified = true,
        //    IsOnBoarded = false,
        //});

        //_dbContext.SaveChanges();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email)
            ?? throw new UserNotFoundException("User not found");

        return new UserResponse(user);
        //return await userService.GetUserByEmailAsync(email);
    }

    [HttpPost("register")]
    public async Task<UserResponse> RegisterNewUser([FromBody] RegisterUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        //return await userService.RegisterNewUser(request);
        throw new NotImplementedException();
    }

    [HttpPut("update")]
    public async Task<UserResponse> UpdateUser([FromBody] UpdateUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        //return await userService.UpdateUserAsync(request);
        throw new NotImplementedException();
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
