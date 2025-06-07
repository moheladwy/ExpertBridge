using ExpertBridge.Api.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly FirebaseAuthService _authService;

    public AuthController(FirebaseAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<string> Login([FromBody] LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return await _authService.LoginAsync(request.Email, request.Password);
    }

    [HttpPost("register")]
    public async Task<string> Register([FromBody] RegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return await _authService.RegisterAsync(request.Email, request.Password);
    }
}
