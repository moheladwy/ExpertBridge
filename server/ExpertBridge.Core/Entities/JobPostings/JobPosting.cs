using ExpertBridge.Core.Entities.Media.JobPostingMedia;

namespace ExpertBridge.Core.Entities.JobPostings;

public class JobPosting : BaseModel, ISoftDeletable
{
    // Properties
    public string Title { get; set; }
    public string Description { get; set; }
    public double Cost { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Foreign keys
    public string AuthorId { get; set; }
    public string AreaId { get; set; }
    public string CategoryId { get; set; }

    // Navigation properties
    public Profiles.Profile Author { get; set; }
    public Areas.Area Area { get; set; }
    public JobCategories.JobCategory Category { get; set; }
    public Jobs.Job? Job { get; set; }
    public ICollection<JobPostingMedia> Medias { get; set; } = [];
}
