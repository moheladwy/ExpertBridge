

using ExpertBridge.Core.Entities.CommentVotes;
using ExpertBridge.Core.Entities.Media.CommentMedia;
using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.Comments;

public class Comment : BaseModel, ISoftDeletable
{
    public required string AuthorId { get; set; }
    public required string PostId { get; set; }
    public required string Content { get; set; }
    public string? ParentCommentId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation property
    public Profile Author { get; set; }
    public Posts.Post Post { get; set; }
    public Comment ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = [];
    public ICollection<CommentVote> Votes { get; set; } = [];
    public CommentMedia Media { get; set; }
}
