using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ProfileBadge;

namespace ExpertBridge.Api.Core.Entities.Badge;

public class Badge
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<ProfileBadge> ProfileBadges { get; set; } = [];
}
