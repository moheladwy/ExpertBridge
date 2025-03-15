using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ProfileSkill;

namespace ExpertBridge.Api.Core.Entities.Skill;

public class Skill
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<ProfileSkill> ProfileSkills { get; set; } = [];
}
