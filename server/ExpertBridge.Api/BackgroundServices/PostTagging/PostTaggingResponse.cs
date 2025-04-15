namespace ExpertBridge.Api.BackgroundServices.PostTagging;

internal class PostTaggingRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
}

internal class PostTaggingResponse
{
    public string Language { get; set; } = string.Empty;
    public List<Tag> Tags { get; set; } = [];
}

internal class Tag
{
    public string EnglishName { get; set; } = string.Empty;
    public string ArabicName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
