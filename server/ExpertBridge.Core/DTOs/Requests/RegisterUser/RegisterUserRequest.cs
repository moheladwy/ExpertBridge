namespace ExpertBridge.Core.DTOs.Requests.RegisterUser;

public class RegisterUserRequest
{
    public string FirebaseId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
