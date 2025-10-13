using System.ComponentModel.DataAnnotations;

namespace ExpertBridge.Core.Requests.JobPostings;

public class CreateJobPostingRequest
{
    public required string Area { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Budget must be non-negative.")]
    public decimal Budget { get; set; }

    public List<MediaObjectRequest>? Media { get; set; }
}
