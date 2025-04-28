




namespace ExpertBridge.Core.Responses;

public record PostResponse
{
    public string Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public AuthorResponse? Author { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public int Comments { get; set; }
    public bool IsUpvoted { get; set; }
    public bool IsDownvoted {  get; set; }
    public List<MediaObjectResponse>? Medias { get; set; }
}
