namespace ExpertBridge.Core.Requests;

public class SearchUserRequest
{
    public required string query { get; set; }
    public int? limit { get; set; }
}
