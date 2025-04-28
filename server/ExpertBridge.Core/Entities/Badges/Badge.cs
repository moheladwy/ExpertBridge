

using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileBadges;

namespace ExpertBridge.Core.Entities.Badges;

public class Badge
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<ProfileBadge> ProfileBadges { get; set; } = [];
}
