namespace ExpertBridge.Core.Entities.Comment;

public class Comment
{
    public string Id { get; set; }
    public string AuthorId { get; set; }
    public string? ParentId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
    public bool isDeleted { get; set; }

    // Navigation property
    public Profile.Profile Author { get; set; }
    public Comment Parent { get; set; }
    public ICollection<Comment> Replies { get; set; } = [];
    public ICollection<Vote.Vote> Votes { get; set; } = [];
    public Media.Media Media { get; set; }
}
