namespace ExpertBridge.Api.Responses
{
    public class ProfileSummaryResponse
    {
        public string ProfileId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Headline { get; set; }
    }
}