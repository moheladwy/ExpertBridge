using System;

namespace ExpertBridge.Core.Requests.JobPostings;

public class ApplyToJobPostingRequest
{
    public required string JobPostingId { get; set; }
    public string? CoverLetter { get; set; }
    public required decimal OfferedCost { get; set; }
}
