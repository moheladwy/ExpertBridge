namespace ExpertBridge.Core.Entities.Comment;

public class Comment
{
    public string Id { get; set; }

    public string AuthorId { get; set; }

    public string ParentId { get; set; }

    public string Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModified { get; set; }

    public bool isDeleted { get; set; }
}
