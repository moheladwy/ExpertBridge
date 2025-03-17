namespace ExpertBridge.Api.Core.Entities.Votes.PostVote;

public class PostVote
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public bool IsUpvote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign keys
    public string ProfileId { get; set; }
    public string PostId { get; set; }

    // Navigation properties
    public Profile.Profile Profile { get; set; }
    public Post.Post Post { get; set; }
}
