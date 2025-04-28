

using ExpertBridge.Core.Entities.Comments;

namespace ExpertBridge.Core.Entities.Media.CommentMedia;

public class CommentMedia : MediaObject
{
    // Foreign keys
    public string CommentId { get; set; }

    // Navigation properties
    public Comment Comment { get; set; }
}
