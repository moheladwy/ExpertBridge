using System;

namespace ExpertBridge.Core.Requests.JobPostings;

public class CreateJobPostingRequest
{
    public required string Area { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required decimal Budget { get; set; }
    public List<MediaObjectRequest>? Media { get; set; }
}
