

using ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileTags;

namespace ExpertBridge.Core.Entities.Tags;

public class Tag
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<ProfileTag> ProfileTags { get; set; } = [];
    public ICollection<PostTag> PostTags { get; set; } = [];
}
