// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.CommentVotes;
using ExpertBridge.Api.Core.Entities.Media.CommentMedia;

namespace ExpertBridge.Api.Core.Entities.Comments;

public class Comment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AuthorId { get; set; }
    public string PostId { get; set; }
    public string ParentCommentId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModified { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation property
    public Profiles.Profile Author { get; set; }
    public Posts.Post Post { get; set; }
    public Comment ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = [];
    public ICollection<CommentVote> Votes { get; set; } = [];
    public CommentMedia Media { get; set; }
}
