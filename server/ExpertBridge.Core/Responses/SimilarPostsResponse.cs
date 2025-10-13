namespace ExpertBridge.Core.Responses;

public class SimilarPostsResponse
{
    public string PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string AuthorName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public double RelevanceScore { get; set; }
}
