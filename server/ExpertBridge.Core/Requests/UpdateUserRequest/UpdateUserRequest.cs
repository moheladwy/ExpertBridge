

namespace ExpertBridge.Core.Requests.UpdateUserRequest;

public class UpdateUserRequest
{
    public required string ProviderId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsEmailVerified { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool IsOnboarded { get; set; }
    public string? Token { get; set; }
}
