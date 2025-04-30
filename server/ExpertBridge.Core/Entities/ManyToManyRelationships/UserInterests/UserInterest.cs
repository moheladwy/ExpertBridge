

using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Tags;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.UserInterests;

public class UserInterest
{
    public string ProfileId { get; set; }
    public string TagId { get; set; }

    // Navigation properties
    public Profile Profile { get; set; }
    public Tag Tag { get; set; }
}
