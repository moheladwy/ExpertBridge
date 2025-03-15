namespace ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ProfileBadge;

public class ProfileBadge
{
    public string ProfileId { get; set; }
    public Profile.Profile Profile { get; set; }

    public string BadgeId { get; set; }
    public Badge.Badge Badge { get; set; }
}
