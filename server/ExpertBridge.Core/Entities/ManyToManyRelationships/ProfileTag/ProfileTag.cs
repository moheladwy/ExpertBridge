namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileTag;

public class ProfileTag
{
    public string ProfileId { get; set; }
    public string TagId { get; set; }

    // Navigation properties
    public Profile.Profile Profile { get; set; }
    public Tags.Tag Tag { get; set; }
}
