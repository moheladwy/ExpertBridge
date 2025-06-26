using System;

namespace ExpertBridge.Core.Requests.JobPosting;

public class CreateJobPostingRequest
{
    public string AreaId { get; set; }
    public string CategoryId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public double Cost { get; set; }
}
