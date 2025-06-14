using ExpertBridge.Core.Entities.Media.ProfileExperienceMedia;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.ProfileExperiences;

public class ProfileExperience : BaseModel, ISoftDeletable
{
    public string ProfileId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Company { get; set; }
    public string Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public Profiles.Profile Profile { get; set; }
    public ICollection<ProfileExperienceMedia> Medias { get; set; } = [];
}
