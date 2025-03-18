// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.Profiles;

namespace ExpertBridge.Api.Core.Entities.Jobs.JobReviews;

public class JobReview
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModified { get; set; }
    public bool IsDeleted { get; set; }

    // Foreign keys
    public string WorkerId { get; set; }
    public string CustomerId { get; set; }
    public string JobId { get; set; }

    // Navigation properties
    public Profile Worker { get; set; }
    public Profile Customer { get; set; }
    public Job Job { get; set; }
}
