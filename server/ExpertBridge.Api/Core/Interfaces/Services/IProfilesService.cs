// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.DTOs.Responses;
using ExpertBridge.Api.Core.Entities.Users;

namespace ExpertBridge.Api.Core.Interfaces.Services;

public interface IProfilesService
{
    Task<ProfileResponse> GetProfileAsync(string id);
    Task<ProfileResponse> CreateProfileAsync(User user);
    Task<ProfileResponse> GetProfileByUserIdentityProviderIdAsync(string identityProviderId);
}
