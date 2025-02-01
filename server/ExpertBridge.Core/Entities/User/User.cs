namespace ExpertBridge.Core.Entities.User;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public bool isBanned { get; set; }
    public bool isDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public Profile.Profile Profile { get; set; }
}
