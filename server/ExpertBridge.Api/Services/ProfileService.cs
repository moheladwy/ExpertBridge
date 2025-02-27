using ExpertBridge.Core;
using ExpertBridge.Core.DTOs.Responses;
using ExpertBridge.Core.Entities.Profile;
using ExpertBridge.Core.Entities.User;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Core.Interfaces.Services;

namespace ExpertBridge.Api.Services;

public class ProfileService(IEntityRepository<Profile> profileRepository) : IProfileService
{
    public async Task<ProfileResponse> GetProfileAsync(string id)
    {
        var profile = await profileRepository.GetByIdAsNoTrackingAsync(id)
            ?? throw new ProfileNotFoundException("Profile not found");
        return new ProfileResponse(profile);
    }

    public async Task<ProfileResponse> CreateProfileAsync(User user)
    {
        var profile = new Profile
        {
            Id = Guid.NewGuid().ToString(),
            UserId = user.Id,
            Rating = 0,
            RatingCount = 0
        };
        await profileRepository.AddAsync(profile);
        return new ProfileResponse(profile);
    }

    public async Task<ProfileResponse> GetProfileByUserIdentityProviderIdAsync(string identityProviderId)
    {
        var profile = await profileRepository.GetFirstAsNoTrackingAsync(p => p.UserId == identityProviderId)
            ?? throw new ProfileNotFoundException("Profile not found");
        return new ProfileResponse(profile);
    }
}
