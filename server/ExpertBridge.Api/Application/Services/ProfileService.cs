using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.DTOs.Responses;
using ExpertBridge.Api.Core.Entities.Profile;
using ExpertBridge.Api.Core.Entities.User;
using ExpertBridge.Api.Core.Interfaces.Repositories;
using ExpertBridge.Api.Core.Interfaces.Services;

namespace ExpertBridge.Api.Application.Services;

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
