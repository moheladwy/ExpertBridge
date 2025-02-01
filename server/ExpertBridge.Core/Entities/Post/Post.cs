namespace ExpertBridge.Core.Entities.Post;

public class Post
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; }
    public string Content { get; set; }
    public string AuthorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime LastModified { get; set; }
    public bool isDeleted { get; set; }
}
