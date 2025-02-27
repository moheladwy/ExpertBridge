using ExpertBridge.Core.DTOs.Responses;
using ExpertBridge.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController(
    IProfileService profileService
    ) : ControllerBase
{
    [HttpGet("get/{id}")]
    public async Task<ProfileResponse> GetProfile([FromRoute] string id)
    {
        var profile = await profileService.GetProfileAsync(id);
        return profile;
    }

    [HttpGet("get-by-user/{identityProviderId}")]
    public async Task<ProfileResponse> GetProfileByUser([FromRoute] string identityProviderId)
    {
        var profile = await profileService.GetProfileByUserIdentityProviderIdAsync(identityProviderId);
        return profile;
    }
}
