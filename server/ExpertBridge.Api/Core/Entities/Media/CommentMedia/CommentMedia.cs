namespace ExpertBridge.Api.Core.Entities.Media.CommentMedia;

public class CommentMedia
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Foreign keys
    public string CommentId { get; set; }
    public string MediaId { get; set; }

    // Navigation properties
    public Comment.Comment Comment { get; set; }
    public Media Media { get; set; }
}
