using ExpertBridge.Core.Entities.Media.JobPostingMedia;

namespace ExpertBridge.Core.Entities.JobPosting;

public class JobPosting
{
    // Properties
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; }
    public string Description { get; set; }
    public double Cost { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Foreign keys
    public string AuthorId { get; set; }
    public string AreaId { get; set; }
    public string CategoryId { get; set; }

    // Navigation properties
    public Profile.Profile Author { get; set; }
    public Area.Area Area { get; set; }
    public JobCategory.JobCategory Category { get; set; }
    public Job.Job? Job { get; set; }
    public ICollection<JobPostingMedia> Medias { get; set; } = [];
}
