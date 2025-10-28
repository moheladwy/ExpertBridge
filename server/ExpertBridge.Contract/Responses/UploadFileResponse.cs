namespace ExpertBridge.Contract.Responses;

public sealed class UploadFileResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string FileUrl { get; set; }
    public string Key { get; set; }
}
