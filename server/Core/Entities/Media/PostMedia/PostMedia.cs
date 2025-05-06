using Core.Entities.Posts;

namespace Core.Entities.Media.PostMedia;

public class PostMedia : MediaObject
{
    // Foreign keys
    public string PostId { get; set; }

    // Navigation properties
    public Post Post { get; set; }
}
