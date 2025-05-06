

namespace Core.Responses;

public record CommentResponse
{
    public required string Id { get; set; }
    public AuthorResponse? Author { get; set; }
    public required string AuthorId { get; set; }
    public required string PostId { get; set; }
    public string? ParentCommentId { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CommentResponse> Replies { get; set; } = [];
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public bool IsUpvoted { get; set; }
    public bool IsDownvoted { get; set; }
    public List<MediaObjectResponse>? Medias { get; set; }
}
