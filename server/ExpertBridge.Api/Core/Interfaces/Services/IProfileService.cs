using ExpertBridge.Api.Core.DTOs.Responses;
using ExpertBridge.Api.Core.Entities.User;

namespace ExpertBridge.Api.Core.Interfaces.Services;

public interface IProfileService
{
    Task<ProfileResponse> GetProfileAsync(string id);
    Task<ProfileResponse> CreateProfileAsync(User user);
    Task<ProfileResponse> GetProfileByUserIdentityProviderIdAsync(string identityProviderId);
}
