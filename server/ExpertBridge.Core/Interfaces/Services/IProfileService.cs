using ExpertBridge.Core.DTOs.Responses;
using ExpertBridge.Core.Entities.User;

namespace ExpertBridge.Core.Interfaces.Services;

public interface IProfileService
{
    Task<ProfileResponse> GetProfileAsync(string id);
    Task<ProfileResponse> CreateProfileAsync(User user);
    Task<ProfileResponse> GetProfileByUserIdentityProviderIdAsync(string identityProviderId);
}
