

using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkills;

namespace ExpertBridge.Core.Entities.Skills;

public class Skill
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<ProfileSkill> ProfileSkills { get; set; } = [];
}
