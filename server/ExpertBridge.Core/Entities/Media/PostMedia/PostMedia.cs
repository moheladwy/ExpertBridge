namespace ExpertBridge.Core.Entities.Media.PostMedia;

public class PostMedia
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Foreign keys
    public string PostId { get; set; }
    public string MediaId { get; set; }

    // Navigation properties
    public Post.Post Post { get; set; }
    public Media Media { get; set; }
}
