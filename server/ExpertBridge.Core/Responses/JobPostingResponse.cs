using System;

namespace ExpertBridge.Core.Responses;

public class JobPostingResponse
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string AreaId { get; set; }
    public string CategoryId { get; set; }
    public double Cost { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ProfileSummaryResponse AuthorProfile { get; set; }
}
