using Core.Entities.ManyToManyRelationships.PostTags;
using Core.Entities.ManyToManyRelationships.UserInterests;

namespace Core.Entities.Tags;

public class Tag : BaseModel
{
    public string EnglishName { get; set; }
    public string? ArabicName { get; set; }
    public string Description { get; set; }

    public ICollection<UserInterest> ProfileTags { get; set; } = [];
    public ICollection<PostTag> PostTags { get; set; } = [];
}
