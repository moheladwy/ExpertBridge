namespace ExpertBridge.Api.Core.DTOs.Responses;

public class GetFileResponse
{
    public Stream ResponseStream { get; set; }
    public string ContentType { get; set; }
    public string FileName { get; set; }
}
