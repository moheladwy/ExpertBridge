using Core.Entities.ProfileExperiences;

namespace Core.Entities.Media.ProfileExperienceMedia;

public class ProfileExperienceMedia : MediaObject
{

    // Foreign keys
    public string ProfileExperienceId { get; set; }

    // Navigation properties
    public ProfileExperience ProfileExperience { get; set; }
}
