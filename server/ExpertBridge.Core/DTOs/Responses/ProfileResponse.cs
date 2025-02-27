using ExpertBridge.Core.Entities.Profile;

namespace ExpertBridge.Core.DTOs.Responses;

public class ProfileResponse
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string? JobTitle { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public int Rating { get; set; }
    public int RatingCount { get; set; }

    public ProfileResponse(Profile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        Id = profile.Id;
        UserId = profile.UserId;
        JobTitle = profile.JobTitle;
        Bio = profile.Bio;
        ProfilePictureUrl = profile.ProfilePictureUrl;
        Rating = profile.Rating;
        RatingCount = profile.RatingCount;
    }
}
