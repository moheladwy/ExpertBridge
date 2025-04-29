

namespace ExpertBridge.Core.Responses
{
    public class MediaObjectResponse
    {
        public string Id { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required string Url { get; set; }
    }
}
