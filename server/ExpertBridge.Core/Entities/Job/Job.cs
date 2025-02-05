namespace ExpertBridge.Core.Entities.Job;

public class Job
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public double ActualCost { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.Now;
    public DateTime? EndedAt { get; set; }

    // Foreign keys
    public string JobStatusId { get; set; }
    public string WorkerId { get; set; }
    public string AuthorId { get; set; }
    public string JobPostingId { get; set; }

    // Navigation properties
    public JobStatus.JobStatus Status { get; set; }
    public JobReview.JobReview Review { get; set; }
    public JobPosting.JobPosting JobPosting { get; set; }
    public Profile.Profile Author { get; set; }
    public Profile.Profile Worker { get; set; }
}
