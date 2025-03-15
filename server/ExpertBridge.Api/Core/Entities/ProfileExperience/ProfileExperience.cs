using ExpertBridge.Api.Core.Entities.Media.ProfileExperienceMedia;

namespace ExpertBridge.Api.Core.Entities.ProfileExperience;

public class ProfileExperience
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProfileId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Company { get; set; }
    public string Location { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }

    // Navigation properties
    public Profile.Profile Profile { get; set; }
    public ICollection<ProfileExperienceMedia> Medias { get; set; } = [];
}
