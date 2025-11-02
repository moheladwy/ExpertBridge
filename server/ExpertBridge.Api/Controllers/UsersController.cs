// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using ExpertBridge.Api.Services;
using ExpertBridge.Application.DomainServices;
using ExpertBridge.Contract.Requests.UpdateUserRequest;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    /// Represents the service for handling user-related operations and business logic.
    /// </summary>
    private readonly UserService _userService;

    /// <summary>
    ///     Provides API endpoints for user account management and authentication.
    /// </summary>
    public UsersController(UserService userService)
    {
        _userService = userService;
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

        return await _userService.UpdateUserAsync(request);
    }
}
