namespace ExpertBridge.Core.Entities.ProfileExperience;

public class ProfileExperience
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Company { get; set; }
    public string Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Navigation properties
    public Profile.Profile Profile { get; set; }
}
