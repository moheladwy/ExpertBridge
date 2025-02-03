namespace ExpertBridge.Core.Entities.Media.ProfileMedia;

public class ProfileMedia
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Foreign keys
    public string ProfileId { get; set; }
    public string MediaId { get; set; }

    // Navigation properties
    public Profile.Profile Profile { get; set; }
    public Media Media { get; set; }
}
