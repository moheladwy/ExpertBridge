using ExpertBridge.Core.Entities.JobPostings;

namespace ExpertBridge.Core.Entities.Media.JobPostingMedia;

public class JobPostingMedia : MediaObject
{
    // Foreign keys
    public string JobPostingId { get; set; }

    // Navigation properties
    public JobPosting JobPosting { get; set; }
}
