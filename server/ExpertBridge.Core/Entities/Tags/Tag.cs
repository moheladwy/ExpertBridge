using ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileTags;

namespace ExpertBridge.Core.Entities.Tags;

public class Tag : BaseModel
{
    public string EnglishName { get; set; }
    public string ArabicName { get; set; }
    public string Description { get; set; }

    public ICollection<ProfileTag> ProfileTags { get; set; } = [];
    public ICollection<PostTag> PostTags { get; set; } = [];
}
