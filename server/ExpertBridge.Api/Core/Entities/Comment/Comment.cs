using ExpertBridge.Api.Core.Entities.Media.CommentMedia;
using ExpertBridge.Api.Core.Entities.Votes.CommentVote;
using ExpertBridge.Api.Core.Entities.Post;

namespace ExpertBridge.Api.Core.Entities.Comment;

public class Comment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AuthorId { get; set; }
    public string PostId { get; set; }
    public string ParentCommentId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModified { get; set; }
    public bool isDeleted { get; set; }

    // Navigation property
    public Profile.Profile Author { get; set; }
    public Post.Post Post { get; set; }
    public Comment ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = [];
    public ICollection<CommentVote> Votes { get; set; } = [];
    public CommentMedia Media { get; set; }
}
