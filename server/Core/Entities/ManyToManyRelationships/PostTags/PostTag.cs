

namespace Core.Entities.ManyToManyRelationships.PostTags;

public class PostTag
{
    public string PostId { get; set; }
    public string TagId { get; set; }

    // Navigation properties
    public Posts.Post Post { get; set; }
    public Tags.Tag Tag { get; set; }
}
