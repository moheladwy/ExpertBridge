using Core.Entities.Jobs;
using Core.Entities.Profiles;

namespace Core.Entities.JobReviews;

public class JobReview : BaseModel
{
    public string Content { get; set; }
    public int Rating { get; set; }
    public bool IsDeleted { get; set; }

    // Foreign keys
    public string WorkerId { get; set; }
    public string CustomerId { get; set; }
    public string JobId { get; set; }

    // Navigation properties
    public Profile Worker { get; set; }
    public Profile Customer { get; set; }
    public Job Job { get; set; }
}
