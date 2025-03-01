namespace ExpertBridge.Core.DTOs.Requests.UpdateUserRequest;

public class UpdateUserRequest
{
    public string ProviderId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string? PhoneNumber { get; set; }
}
