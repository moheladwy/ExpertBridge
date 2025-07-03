// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.JobReviews;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.Jobs;

public class Job : BaseModel, ISoftDeletable
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public double ActualCost { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    //public bool IsPaid { get; set; }
    //public bool IsCompleted { get; set; }

    public JobStatuses.JobStatusEnum Status { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Foreign keys

    public string AuthorId { get; set; }
    public string WorkerId { get; set; }
    public string? JobPostingId { get; set; }

    // Navigation properties
    public JobReview Review { get; set; }
    public JobPosting JobPosting { get; set; }
    public Profile Author { get; set; }
    public Profile Worker { get; set; }
}
