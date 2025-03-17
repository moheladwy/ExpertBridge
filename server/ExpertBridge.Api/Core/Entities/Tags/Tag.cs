using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.PostTag;
using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ProfileTag;

namespace ExpertBridge.Api.Core.Entities.Tags;

public class Tag
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<ProfileTag> ProfileTags { get; set; } = [];
    public ICollection<PostTag> PostTags { get; set; } = [];
}
