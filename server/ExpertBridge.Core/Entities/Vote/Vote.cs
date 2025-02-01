namespace ExpertBridge.Core.Entities.Vote;

public class Vote
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProfileId { get; set; }
    public bool IsUpvote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Profile.Profile Profile { get; set; }
}
