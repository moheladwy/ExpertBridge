namespace ExpertBridge.Core.Entities.Vote.PostVote;

public class PostVote : Vote
{
    public string? PostId { get; set; }

    // Navigation properties
    public Post.Post? Post { get; set; }
}
