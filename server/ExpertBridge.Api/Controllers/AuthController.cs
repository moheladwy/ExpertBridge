// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IFirebaseAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<string> Login([FromBody] LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return await authService.LoginAsync(request.Email, request.Password);
    }

    [HttpPost("register")]
    public async Task<string> Register([FromBody] RegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return await authService.RegisterAsync(request.Email, request.Password);
    }
}
