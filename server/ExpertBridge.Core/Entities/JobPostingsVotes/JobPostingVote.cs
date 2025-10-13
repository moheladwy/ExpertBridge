// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.JobPostingsVotes;

public class JobPostingVote : BaseModel
{
    public bool IsUpvote { get; set; }

    // Foreign keys
    public string ProfileId { get; set; }
    public string JobPostingId { get; set; }

    // Navigation properties
    public Profile Profile { get; set; }
    public JobPosting JobPosting { get; set; }
}
