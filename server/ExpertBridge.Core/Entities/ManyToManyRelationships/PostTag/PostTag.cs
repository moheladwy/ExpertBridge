namespace ExpertBridge.Core.Entities.ManyToManyRelationships.PostTag;

public class PostTag
{
    public string PostId { get; set; }
    public string TagId { get; set; }

    // Navigation properties
    public Post.Post Post { get; set; }
    public Tags.Tag Tag { get; set; }
}
