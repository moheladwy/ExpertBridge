using ExpertBridge.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IFirebaseService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<string> Login([FromBody] LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        var result = await authService.LoginAsync(request.Email, request.Password);
        return result;
    }

    [HttpPost("register")]
    public async Task<string> Register([FromBody] RegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        var result = await authService.RegisterAsync(request.Email, request.Password);
        return result;
    }
}
