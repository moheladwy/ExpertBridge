

using System.Text.Json.Serialization;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.Users;

public class User : BaseModel, ISoftDeletable
{
    public required string ProviderId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Email { get; set; }
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsBanned { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsOnboarded { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    [JsonIgnore]
    public Profile Profile { get; set; }
}
