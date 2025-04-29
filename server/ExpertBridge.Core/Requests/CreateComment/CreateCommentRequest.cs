

namespace ExpertBridge.Core.Requests.CreateComment;

public class CreateCommentRequest
{
    public required string PostId { get; set; }
    public string? ParentCommentId { get; set; }
    public required string Content { get; set; }
    public List<string>? MediaUrls { get; set; }
}
