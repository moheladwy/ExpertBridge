using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.PostTag;
using ExpertBridge.Api.Core.Entities.Media.PostMedia;
using ExpertBridge.Api.Core.Entities.Votes.PostVote;

namespace ExpertBridge.Api.Core.Entities.Post;

public class Post
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; }
    public string Content { get; set; }
    public string AuthorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModified { get; set; }
    public bool isDeleted { get; set; }

    // Add to navigation properties
    public Profile.Profile Author { get; set; }
    public ICollection<PostMedia> Medias { get; set; } = [];
    public ICollection<Comment.Comment> Comments { get; set; } = [];
    public ICollection<PostVote> Votes { get; set; } = [];
    public ICollection<PostTag> PostTags { get; set; } = [];
}
