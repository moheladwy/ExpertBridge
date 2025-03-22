//// Licensed to the .NET Foundation under one or more agreements.
//// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.Interfaces.Services;
using ExpertBridge.Api.Data.DatabaseContexts;
using ExpertBridge.Api.Queries;
using ExpertBridge.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly ExpertBridgeDbContext _dbContext;

    public ProfilesController(ExpertBridgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ProfileResponse> GetProfile()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            throw new UnauthorizedException();
        }

        var profile = await _dbContext.Profiles
            .Where(p => p.UserId == user.Id)
            .Include(p => p.User)
            .SelectProfileResponseFromProfile()
            .FirstOrDefaultAsync();

        if (profile == null)
        {
            throw new ProfileNotFoundException($"User[{user.Id}] Profile was not found");
        }

        return profile;
    }

    [HttpGet("get-by-user/{identityProviderId}")]
    public async Task<ProfileResponse> GetProfileByUser([FromRoute] string identityProviderId)
    {
        //ArgumentException.ThrowIfNullOrEmpty(identityProviderId, nameof(identityProviderId));
        //return await profileService.GetProfileByUserIdentityProviderIdAsync(identityProviderId);
        throw new NotImplementedException();
    }
}
