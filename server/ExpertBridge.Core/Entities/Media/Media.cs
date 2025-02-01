namespace ExpertBridge.Core.Entities.Media;

public class Media
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string MediaUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }

    // Foreign keys
    public string? ProfileId { get; set; }
    public string? PostId { get; set; }
    public string? JobPostingId { get; set; }
    public string? ChatId { get; set; }
    public string? ProfileExperienceId { get; set; }
    public string? CommentId { get; set; }
    public string MediaTypeId { get; set; }

    // Navigation properties
    public MediaType.MediaType MediaType { get; set; }
    public Profile.Profile Profile { get; set; }
    public Post.Post Post { get; set; }
    public JobPosting.JobPosting JobPosting { get; set; }
    public Chat.Chat Chat { get; set; }
    public ProfileExperience.ProfileExperience ProfileExperience { get; set; }
    public Comment.Comment Comment { get; set; }
}
