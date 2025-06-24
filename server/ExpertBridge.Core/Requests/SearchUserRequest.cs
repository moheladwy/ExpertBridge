namespace ExpertBridge.Core.Requests;

public class SearchUserRequest
{
    public required string Query { get; set; }
    public int? Limit { get; set; }
}
