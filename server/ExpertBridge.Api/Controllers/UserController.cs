using System.Security.Claims;
using ExpertBridge.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Core.DTOs.Responses;
using ExpertBridge.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("get/{identityProviderId}")]
    public async Task<UserResponse> GetUserByIdentityProviderId([FromRoute] string identityProviderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityProviderId);
        return await userService.GetUserByIdentityProviderId(identityProviderId);
    }

    [HttpPost("register")]
    public async Task<UserResponse> RegisterNewUser([FromBody] RegisterUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
        if (email != request.Email)
            throw new UnauthorizedAccessException("Email does not match the authenticated user.");

        return await userService.RegisterNewUser(request);
    }

}
