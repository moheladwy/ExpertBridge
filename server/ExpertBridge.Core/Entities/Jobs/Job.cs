// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Core.Entities.Jobs;

public class Job
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public double ActualCost { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    // Foreign keys
    public string JobStatusId { get; set; }
    public string WorkerId { get; set; }
    public string AuthorId { get; set; }
    public string JobPostingId { get; set; }

    // Navigation properties
    public Entities.JobStatuses.JobStatus Status { get; set; }
    public JobReviews.JobReview Review { get; set; }
    public JobPostings.JobPosting JobPosting { get; set; }
    public Profiles.Profile Author { get; set; }
    public Profiles.Profile Worker { get; set; }
}
