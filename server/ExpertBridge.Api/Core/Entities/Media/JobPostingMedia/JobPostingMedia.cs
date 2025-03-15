namespace ExpertBridge.Api.Core.Entities.Media.JobPostingMedia;

public class JobPostingMedia
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Foreign keys
    public string JobPostingId { get; set; }
    public string MediaId { get; set; }

    // Navigation properties
    public JobPosting.JobPosting JobPosting { get; set; }
    public Media Media { get; set; }
}
