// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

// using System.Numerics;

using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags;
using ExpertBridge.Core.Entities.Media.PostMedia;
using ExpertBridge.Core.Entities.PostVotes;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;
using Pgvector;

namespace ExpertBridge.Core.Entities.Posts;

public class Post : BaseModel, ISoftDeletable, IRecommendableContent, ISafeContent
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string AuthorId { get; set; }
    public string? Language { get; set; }
    public bool IsTagged { get; set; }
    public bool IsSafeContent { get; set; }
    public bool IsProcessed { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // this is the date when the author updated the post last time.
    // it is different from LastModified, which is set by the system.
    public DateTime? UpdatedAt { get; set; }

    // [Column(TypeName = "vector(1024)")]
    public Vector? Embedding { get; set; }

    // Add to navigation properties
    public Profile Author { get; set; }
    public ICollection<PostMedia> Medias { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<PostVote> Votes { get; set; } = [];
    public ICollection<PostTag> PostTags { get; set; } = [];
}
