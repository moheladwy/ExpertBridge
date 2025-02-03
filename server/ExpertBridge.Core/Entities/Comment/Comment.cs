using ExpertBridge.Core.Entities.Media.CommentMedia;
using ExpertBridge.Core.Entities.Votes.CommentVote;

namespace ExpertBridge.Core.Entities.Comment;

public class Comment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AuthorId { get; set; }
    public string? ParentId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
    public bool isDeleted { get; set; }

    // Navigation property
    public Profile.Profile Author { get; set; }
    public Comment Parent { get; set; }
    public ICollection<Comment> Replies { get; set; } = [];
    public ICollection<CommentVote> Votes { get; set; } = [];
    public CommentMedia Media { get; set; }
}
