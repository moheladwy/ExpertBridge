namespace ExpertBridge.Api.Requests.EditComment;

public class EditCommentRequest
{
    public required string Id { get; set; }
    public string? Content { get; set; }
}
