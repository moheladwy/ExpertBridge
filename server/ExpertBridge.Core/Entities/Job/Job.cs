namespace ExpertBridge.Core.Entities.Job;

public class Job
{
    public string Id { get; set; }

    public double ActualCost { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime EndedAt { get; set; }

    public JobStatus Status { get; set; }

    public string WorkerId { get; set; }

    public string AuthoerId { get; set; }

    public string JobPostingId { get; set; }
}
