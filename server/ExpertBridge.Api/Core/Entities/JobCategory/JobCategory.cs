namespace ExpertBridge.Api.Core.Entities.JobCategory;

public class JobCategory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<JobPosting.JobPosting> JobPostings { get; set; } = [];
}
