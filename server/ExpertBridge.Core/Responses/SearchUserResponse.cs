namespace ExpertBridge.Core.Responses;

public class SearchUserResponse
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? JobTitle { get; set; }
    public string? Bio { get; set; }
    public double Rank { get; set; }
}
