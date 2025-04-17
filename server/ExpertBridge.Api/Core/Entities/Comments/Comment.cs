// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.CommentVotes;
using ExpertBridge.Api.Core.Entities.Media.CommentMedia;
using ExpertBridge.Api.Core.Entities.Profiles;

namespace ExpertBridge.Api.Core.Entities.Comments;

public class Comment : BaseModel
{
    public required string AuthorId { get; set; }
    public required string PostId { get; set; }
    public required string Content { get; set; }
    public string? ParentCommentId { get; set; }

    // Navigation property
    public Profile Author { get; set; }
    public Posts.Post Post { get; set; }
    public Comment ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = [];
    public ICollection<CommentVote> Votes { get; set; } = [];
    public CommentMedia Media { get; set; }
}
