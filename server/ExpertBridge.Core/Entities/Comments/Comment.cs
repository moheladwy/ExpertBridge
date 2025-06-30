// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.CommentVotes;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.Media.CommentMedia;
using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.Comments;

public class Comment : BaseModel, ISoftDeletable
{
    public required string AuthorId { get; set; }
    public string? PostId { get; set; }
    public string? JobPostingId { get; set; }
    public required string Content { get; set; }
    public string? ParentCommentId { get; set; }
    public bool IsProcessed { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public Profile Author { get; set; }
    public Post? Post { get; set; }
    public JobPosting? JobPosting { get; set; }
    public Comment ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = [];
    public ICollection<CommentVote> Votes { get; set; } = [];
    public ICollection<CommentMedia> Medias { get; set; } = [];
}
