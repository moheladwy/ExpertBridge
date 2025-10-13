using System.ComponentModel.DataAnnotations;

namespace ExpertBridge.Core.Requests.JobPostings;

public class ApplyToJobPostingRequest
{
    public required string JobPostingId { get; set; }
    public string? CoverLetter { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "OfferedCost must be non-negative.")]
    public required decimal OfferedCost { get; set; }
}
