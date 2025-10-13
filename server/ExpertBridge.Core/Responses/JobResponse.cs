// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobStatuses;

namespace ExpertBridge.Core.Responses;

public class JobResponse
{
    public string Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string ChatId { get; set; }
    public decimal ActualCost { get; set; }
    public string Area { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    public bool IsPaid { get; set; }
    public bool IsCompleted { get; set; }

    public JobStatusEnum Status { get; set; }
    public DateTime? UpdatedAt { get; set; }


    public required string AuthorId { get; set; }
    public required string WorkerId { get; set; }

    // Navigation properties
    public AuthorResponse Author { get; set; }
    public AuthorResponse Worker { get; set; }
}
