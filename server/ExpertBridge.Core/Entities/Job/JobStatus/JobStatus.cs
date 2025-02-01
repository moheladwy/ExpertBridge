namespace ExpertBridge.Core.Entities.Job.JobStatus;

public class JobStatus
{
    public string Id { get; set; }
    public JobStatusEnum Status { get; set; }

    // Navigation properties
    public ICollection<Job> Jobs { get; set; } = [];
}
