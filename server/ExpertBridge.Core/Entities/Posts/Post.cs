

// using System.Numerics;
using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags;
using ExpertBridge.Core.Entities.Media.PostMedia;
using ExpertBridge.Core.Entities.PostVotes;
using ExpertBridge.Core.Entities.Profiles;
using Pgvector;

namespace ExpertBridge.Core.Entities.Posts;

public class Post : BaseModel, ISoftDeletable
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string AuthorId { get; set; }
    public bool IsTagged { get; set; }
    public bool IsProcessed { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // [Column(TypeName = "vector(1024)")]
    public Vector? Embeddings { get; set; }

    // Add to navigation properties
    public Profile Author { get; set; }
    public ICollection<PostMedia> Medias { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<PostVote> Votes { get; set; } = [];
    public ICollection<PostTag> PostTags { get; set; } = [];
}
