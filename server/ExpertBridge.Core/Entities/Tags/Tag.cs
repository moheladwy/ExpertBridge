namespace ExpertBridge.Core.Entities.Tags;

public class Tag
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public ICollection<ManyToManyRelationships.ProfileTag.ProfileTag> ProfileTags { get; set; } = [];
    public ICollection<Profile.Profile> Profiles { get; set; } = [];
}
