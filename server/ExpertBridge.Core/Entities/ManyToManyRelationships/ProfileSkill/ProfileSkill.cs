namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkill;

public class ProfileSkill
{
    public string ProfileId { get; set; }
    public string SkillId { get; set; }

    // Navigation properties
    public Profile.Profile Profile { get; set; }
    public Skill.Skill Skill { get; set; }
}
