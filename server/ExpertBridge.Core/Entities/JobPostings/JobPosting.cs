// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.JobApplications;
using ExpertBridge.Core.Entities.JobPostingsVotes;
using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.ManyToManyRelationships.JobPostingTags;
using ExpertBridge.Core.Entities.Media.JobPostingMedia;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;
using Pgvector;

namespace ExpertBridge.Core.Entities.JobPostings;

public class JobPosting : BaseModel, ISoftDeletable, IRecommendableContent, ISafeContent
{
    // Properties
    public string Title { get; set; }
    public string Content { get; set; }
    public string AuthorId { get; set; }
    public string? Language { get; set; }
    public decimal Budget { get; set; }
    public string Area { get; set; }
    public bool IsTagged { get; set; }
    public bool IsProcessed { get; set; }
    public bool IsSafeContent { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Vector? Embedding { get; set; }
    public Profile Author { get; set; }
    public ICollection<JobPostingMedia> Medias { get; set; } = [];
    public ICollection<JobPostingTag> JobPostingTags { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<JobPostingVote> Votes { get; set; } = [];
    public ICollection<JobApplication> JobApplications { get; set; } = [];
    public ICollection<Job> Jobs { get; set; } = [];
}
