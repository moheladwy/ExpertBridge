using Core.Entities.Jobs;

namespace Core.Entities.JobStatuses;

public class JobStatus
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public JobStatusEnum Status { get; set; }

    // Navigation properties
    public ICollection<Job> Jobs { get; set; } = [];
}
