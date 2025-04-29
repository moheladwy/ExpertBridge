

namespace ExpertBridge.Core.Entities.PostVotes;

public class PostVote : BaseModel
{
    public bool IsUpvote { get; set; }

    // Foreign keys
    public string ProfileId { get; set; }
    public string PostId { get; set; }

    // Navigation properties
    public Profiles.Profile Profile { get; set; }
    public Posts.Post Post { get; set; }
}
