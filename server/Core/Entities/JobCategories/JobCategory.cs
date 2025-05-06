

namespace Core.Entities.JobCategories;

public class JobCategory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<JobPostings.JobPosting> JobPostings { get; set; } = [];
}
