namespace ExpertBridge.Core.Entities.JobPosting;

public class JobPosting
{
    public string Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public double Cost { get; set; }

    public string AuthorId { get; set; }

    public string AreaId { get; set; }

    public string CategoryId { get; set; }
}
