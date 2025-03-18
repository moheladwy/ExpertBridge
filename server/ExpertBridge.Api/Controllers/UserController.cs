using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Api.Core.DTOs.Requests.UpdateUserRequest;
using ExpertBridge.Api.Core.DTOs.Responses;
using ExpertBridge.Api.Core.Interfaces.Services;
using ExpertBridge.Api.Data.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UserController(
    IUserService userService,
    ExpertBridgeDbContext _dbContext
    ) : ControllerBase
{
    [HttpGet("get/{identityProviderId}")]
    public async Task<UserResponse> GetUserByIdentityProviderId([FromRoute] string identityProviderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        return await userService.GetUserByIdentityProviderId(identityProviderId);
    }

    [HttpGet("get-by-email/{email}")]
    public async Task<UserResponse> GetUserByEmail([FromRoute] string email)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);

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
        return await userService.RegisterNewUser(request);
    }

    [HttpPut("update")]
    public async Task<UserResponse> UpdateUser([FromBody] UpdateUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await userService.UpdateUserAsync(request);
    }

    [HttpGet("is-deleted/{identityProviderId}")]
    public async Task<bool> IsUserDeleted([FromRoute] string identityProviderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        return await userService.IsUserDeletedAsync(identityProviderId);
    }

    [HttpDelete("delete/{identityProviderId}")]
    public async Task<IActionResult> DeleteUser([FromRoute] string identityProviderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        await userService.DeleteUserAsync(identityProviderId);
        return Ok("User deleted successfully.");
    }

    [HttpGet("is-banned/{identityProviderId}")]
    public async Task<bool> IsUserBanned([FromRoute] string identityProviderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        return await userService.IsUserBannedAsync(identityProviderId);
    }

    [HttpPut("ban/{identityProviderId}")]
    public async Task<IActionResult> BanUser([FromRoute] string identityProviderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        await userService.BanUserAsync(identityProviderId);
        return Ok("User banned successfully.");
    }

    [HttpGet("is-verified/{email}")]
    public async Task<bool> IsUserVerified([FromRoute] string email)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);
        return await userService.IsUserVerifiedAsync(email);
    }

    [HttpPut("verify/{email}")]
    public async Task<IActionResult> VerifyUser([FromRoute] string email)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);
        await userService.VerifyUserAsync(email);
        return Ok("User verified successfully.");
    }
}
