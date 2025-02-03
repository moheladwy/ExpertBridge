namespace ExpertBridge.Core.Entities.Votes.CommentVote;

public class CommentVote
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public bool IsUpvote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign keys
    public string CommentId { get; set; }
    public string ProfileId { get; set; }

    // Navigation properties
    public Profile.Profile Profile { get; set; }
    public Comment.Comment Comment { get; set; }
}
