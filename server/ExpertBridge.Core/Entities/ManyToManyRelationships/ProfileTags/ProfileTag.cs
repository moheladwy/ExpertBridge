

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileTags;

public class ProfileTag
{
    public string ProfileId { get; set; }
    public string TagId { get; set; }

    // Navigation properties
    public Profiles.Profile Profile { get; set; }
    public Tags.Tag Tag { get; set; }
}
