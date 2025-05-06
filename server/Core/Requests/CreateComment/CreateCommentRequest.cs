

namespace Core.Requests.CreateComment;

public class CreateCommentRequest
{
    public required string PostId { get; set; }
    public string? ParentCommentId { get; set; }
    public required string Content { get; set; }
    public List<MediaObjectRequest>? Media { get; set; }
}
