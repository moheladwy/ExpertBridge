

namespace ExpertBridge.Core.Entities.CommentVotes;

public class CommentVote : BaseModel
{
    public bool IsUpvote { get; set; }

    // Foreign keys
    public required string CommentId { get; set; }
    public required string ProfileId { get; set; }

    // Navigation properties
    public Profiles.Profile Profile { get; set; }
    public Comments.Comment Comment { get; set; }
}
