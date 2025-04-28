

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileBadges;

public class ProfileBadge
{
    public string ProfileId { get; set; }
    public Profiles.Profile Profile { get; set; }

    public string BadgeId { get; set; }
    public Badges.Badge Badge { get; set; }
}
