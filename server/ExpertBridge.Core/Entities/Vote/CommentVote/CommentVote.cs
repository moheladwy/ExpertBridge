namespace ExpertBridge.Core.Entities.Vote.CommentVote;

public class CommentVote : Vote
{
    public string? CommentId { get; set; }

    // Navigation properties
    public Comment.Comment Comment { get; set; }
}
