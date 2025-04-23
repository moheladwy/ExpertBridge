using ExpertBridge.Api.Data.DatabaseContexts;
using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Models;
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
    private readonly AuthorizationHelper _authHelper;

    public ProfilesController(ExpertBridgeDbContext dbContext, AuthorizationHelper authHelper)
    {
        _dbContext = dbContext;
        _authHelper = authHelper;
    }

    [HttpGet]
    public async Task<ProfileResponse> GetProfile()
    {
        var user = await _authHelper.GetCurrentUserAsync(User);
        if (user == null) throw new UnauthorizedException();

        var profile = await _dbContext.Profiles
            .AsNoTracking()
            .Where(p => p.UserId == user.Id)
            .Include(p => p.User)
            .SelectProfileResponseFromProfile()
            .FirstOrDefaultAsync();

        if (profile == null)
            throw new ProfileNotFoundException($"User[{user.Id}] Profile was not found");

        return profile;
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ProfileResponse> GetProfile(string id)
    {
        var profile = await _dbContext.Profiles
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Include(p => p.User)
            .SelectProfileResponseFromProfile()
            .FirstOrDefaultAsync();

        if (profile == null)
            throw new ProfileNotFoundException($"Profile with id={id} was not found");

        return profile;
    }
}
