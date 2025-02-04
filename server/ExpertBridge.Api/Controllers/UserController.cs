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
    [HttpGet("get/{firebaseId}")]
    public async Task<UserResponse> GetUserByFirebaseId([FromRoute] string firebaseId)
    {
        ArgumentException.ThrowIfNullOrEmpty(firebaseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(firebaseId);
        return await userService.GetUserByFirebaseId(firebaseId);
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
