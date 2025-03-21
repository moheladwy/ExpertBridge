//// Licensed to the .NET Foundation under one or more agreements.
//// The .NET Foundation licenses this file to you under the MIT license.

//using ExpertBridge.Api.Core.Interfaces.Services;
//using ExpertBridge.Api.Responses;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace ExpertBridge.Api.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//[Authorize]
//public class ProfilesController(
//    IProfilesService profileService
//    ) : ControllerBase
//{
//    [HttpGet("get/{id}")]
//    public async Task<ProfileResponse> GetProfile([FromRoute] string id)
//    {
//        ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));
//        return await profileService.GetProfileAsync(id);
//    }

//    [HttpGet("get-by-user/{identityProviderId}")]
//    public async Task<ProfileResponse> GetProfileByUser([FromRoute] string identityProviderId)
//    {
//        ArgumentException.ThrowIfNullOrEmpty(identityProviderId, nameof(identityProviderId));
//        return await profileService.GetProfileByUserIdentityProviderIdAsync(identityProviderId);
//    }
//}
