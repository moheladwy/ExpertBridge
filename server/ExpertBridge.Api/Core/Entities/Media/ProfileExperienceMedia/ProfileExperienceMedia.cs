namespace ExpertBridge.Api.Core.Entities.Media.ProfileExperienceMedia;

public class ProfileExperienceMedia
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Foreign keys
    public string ProfileExperienceId { get; set; }
    public string MediaId { get; set; }

    // Navigation properties
    public ProfileExperience.ProfileExperience ProfileExperience { get; set; }
    public Media Media { get; set; }
}
