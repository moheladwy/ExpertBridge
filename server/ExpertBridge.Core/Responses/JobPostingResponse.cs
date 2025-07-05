using System;

namespace ExpertBridge.Core.Responses;

public class JobPostingResponse
{
    public string Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? Language { get; set; }
    public AuthorResponse? Author { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public int Comments { get; set; }
    public bool IsUpvoted { get; set; }
    public bool IsDownvoted { get; set; }
    public bool IsAppliedFor { get; set; } 
    public double? RelevanceScore { get; set; }

    public decimal Budget { get; set; }
    public string Area { get; set; }

    public List<TagResponse>? Tags { get; set; }

    public List<MediaObjectResponse>? Medias { get; set; }
}
