using Core.Entities.JobPostings;

namespace Core.Entities.Media.JobPostingMedia;

public class JobPostingMedia : MediaObject
{
    // Foreign keys
    public string JobPostingId { get; set; }

    // Navigation properties
    public JobPosting JobPosting { get; set; }
}
