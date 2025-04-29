

namespace ExpertBridge.Core.Requests.EditComment
{
    public class PatchCommentRequest
    {
        public required string CommentId { get; set; }
        public bool? Upvote { get; set; }
        public bool? Downvote { get; set; }
        public string? Content { get; set; }
    }
}
