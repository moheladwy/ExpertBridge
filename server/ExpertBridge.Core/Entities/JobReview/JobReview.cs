namespace ExpertBridge.Core.Entities.JobReview;

public class JobReview
{
    public string Id { get; set; }

    public string Content { get; set; }

    public string WorkerId { get; set; }

    public string CustomerId { get; set; }

    public string JobId { get; set; }

    public int Rating { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }
}
