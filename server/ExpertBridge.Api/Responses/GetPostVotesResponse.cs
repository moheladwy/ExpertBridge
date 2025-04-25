namespace ExpertBridge.Api.Responses;

public class GetPostVotesResponse
{
    public string PostId { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public bool IsUpvoted { get; set; }
}
