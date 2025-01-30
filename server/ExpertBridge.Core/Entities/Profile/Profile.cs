namespace ExpertBridge.Core.Entities.Profile;

public class Profile
{
    public string Id { get; set; }
    public string JobTitle { get; set; }
    public string Bio { get; set; }
    public int Rating { get; set; }

    // Navigation properties
    public User.User User { get; set; }
    public Area.Area Area { get; set; }
    public List<ProfileExperience.ProfileExperience> Experiences { get; set; }
}
