namespace ExpertBridge.Core.Entities.Area;

public class Area
{
    public string Id { get; set; }
    public string ProfileId { get; set; }
    public string Governorate { get; set; }
    public string Region { get; set; }

    // Navigation properties
    public Profile.Profile Profile { get; set; }
    public ICollection<JobPosting.JobPosting> JobPostings { get; set; } = [];
}
