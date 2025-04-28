

namespace ExpertBridge.Core.Entities.Areas;

public class Area
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProfileId { get; set; }
    public Governorates Governorate { get; set; }
    public string Region { get; set; }

    // Navigation properties
    public Profiles.Profile Profile { get; set; }
    public ICollection<JobPostings.JobPosting> JobPostings { get; set; } = [];
}
