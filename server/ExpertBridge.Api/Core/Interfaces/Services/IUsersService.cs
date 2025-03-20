// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Api.Core.DTOs.Requests.UpdateUserRequest;
using ExpertBridge.Api.Core.DTOs.Responses;

namespace ExpertBridge.Api.Core.Interfaces.Services;

public interface IUsersService
{
    Task<UserResponse> GetUserByIdentityProviderId(string identityProviderId);
    Task<UserResponse> GetUserByEmailAsync(string email);
    Task<UserResponse> RegisterNewUser(RegisterUserRequest request);
    Task<UserResponse> UpdateUserAsync(UpdateUserRequest request);
    Task DeleteUserAsync(string identityProviderId);
    Task<bool> IsUserBannedAsync(string identityProviderId);
    Task<bool> IsUserVerifiedAsync(string email);
    Task<bool> IsUserDeletedAsync(string identityProviderId);
    Task BanUserAsync(string identityProviderId);
    Task VerifyUserAsync(string email);
}
