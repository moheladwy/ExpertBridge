using ExpertBridge.Core.Responses;

namespace ExpertBridge.Application.Models.GroqResponses;

public class TranslateTagsResponse
{
    public List<CategorizerTagResponse> Tags { get; set; }
}
