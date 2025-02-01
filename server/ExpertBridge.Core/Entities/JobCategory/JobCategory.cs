namespace ExpertBridge.Core.Entities.JobCategory;

public class JobCategory
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public ICollection<JobPosting.JobPosting> JobPostings { get; set; } = [];
}
