

namespace ExpertBridge.Core.Requests.CreatePost;

public class CreatePostRequest
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public List<MediaObjectRequest>? Media { get; set; }
}
