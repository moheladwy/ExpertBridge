

namespace Core.Entities.ManyToManyRelationships.ProfileSkills;

public class ProfileSkill
{
    public string ProfileId { get; set; }
    public string SkillId { get; set; }

    // Navigation properties
    public Profiles.Profile Profile { get; set; }
    public Skills.Skill Skill { get; set; }
}
