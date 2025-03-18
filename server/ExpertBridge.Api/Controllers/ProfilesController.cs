using ExpertBridge.Api.Core.DTOs.Responses;
using ExpertBridge.Api.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfilesController(
    IProfileService profileService
    ) : ControllerBase
{
    [HttpGet("get/{id}")]
    public async Task<ProfileResponse> GetProfile([FromRoute] string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));
        return await profileService.GetProfileAsync(id);
    }

    [HttpGet("get-by-user/{identityProviderId}")]
    public async Task<ProfileResponse> GetProfileByUser([FromRoute] string identityProviderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityProviderId, nameof(identityProviderId));
        return await profileService.GetProfileByUserIdentityProviderIdAsync(identityProviderId);
    }
}
