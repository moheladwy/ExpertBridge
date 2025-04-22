namespace ExpertBridge.Api.Requests.EditPost;

public class EditPostRequest
{
    public required string Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
}
