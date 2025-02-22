using ExpertBridge.Core.Entities.Media.ChatMedia;

namespace ExpertBridge.Core.Entities.Media;

public class Media
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string MediaUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModified { get; set; }

    // Foreign keys
    public string MediaTypeId { get; set; }

    // Navigation properties
    public MediaType.MediaType MediaType { get; set; }
    public ProfileMedia.ProfileMedia Profile { get; set; }
    public PostMedia.PostMedia Post { get; set; }
    public ProfileExperienceMedia.ProfileExperienceMedia ProfileExperience { get; set; }
    public CommentMedia.CommentMedia Comment { get; set; }
    public JobPostingMedia.JobPostingMedia JobPosting { get; set; }
    public ChatMedia.ChatMedia Chat { get; set; }
}
