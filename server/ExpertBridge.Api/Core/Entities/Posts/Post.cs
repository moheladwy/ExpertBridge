// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.Comments;
using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.PostTags;
using ExpertBridge.Api.Core.Entities.Media.PostMedia;
using ExpertBridge.Api.Core.Entities.PostVotes;
using ExpertBridge.Api.Core.Entities.Profiles;

namespace ExpertBridge.Api.Core.Entities.Posts;

public class Post : BaseModel
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string AuthorId { get; set; }
    public bool IsTagged { get; set; }
    public bool IsProcessed { get; set; }


    // Add to navigation properties
    public Profile Author { get; set; }
    public ICollection<PostMedia> Medias { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<PostVote> Votes { get; set; } = [];
    public ICollection<PostTag> PostTags { get; set; } = [];
}
