using ExpertBridge.Core.Entities.User;

namespace ExpertBridge.Core.DTOs.Responses;

public class UserResponse
{
    public string Id { get; set; }
    public string FirebaseId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsBanned { get; set; }
    public bool IsDeleted { get; set; }

    public UserResponse(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        Id = user.Id;
        FirebaseId = user.FirebaseId;
        Email = user.Email;
        Username = user.Username;
        FirstName = user.FirstName;
        LastName = user.LastName;
        IsBanned = user.isBanned;
        IsDeleted = user.isDeleted;
    }
}
