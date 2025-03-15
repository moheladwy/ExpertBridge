namespace ExpertBridge.Api.Core.Entities.Job.JobReview;

public class JobReview
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModified { get; set; }
    public bool IsDeleted { get; set; }

    // Foreign keys
    public string WorkerId { get; set; }
    public string CustomerId { get; set; }
    public string JobId { get; set; }

    // Navigation properties
    public Profile.Profile Worker { get; set; }
    public Profile.Profile Customer { get; set; }
    public Job Job { get; set; }
}
