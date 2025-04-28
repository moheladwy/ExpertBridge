

namespace ExpertBridge.Core.Responses
{
    public record AuthorResponse
    {
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? JobTitle { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
