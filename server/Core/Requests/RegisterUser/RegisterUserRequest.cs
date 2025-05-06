

namespace Core.Requests.RegisterUser;

public class RegisterUserRequest
{
    public string ProviderId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
