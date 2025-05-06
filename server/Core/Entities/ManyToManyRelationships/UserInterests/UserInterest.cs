

using Core.Entities.Profiles;
using Core.Entities.Tags;

namespace Core.Entities.ManyToManyRelationships.UserInterests;

public class UserInterest
{
    public string ProfileId { get; set; }
    public string TagId { get; set; }

    // Navigation properties
    public Profile Profile { get; set; }
    public Tag Tag { get; set; }
}
