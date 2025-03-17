using ExpertBridge.Api.Core.Entities.Media.ChatMedia;

namespace ExpertBridge.Api.Core.Entities.Chat;

public class Chat
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }

    // Navigation properties
    public ICollection<ChatParticipant.ChatParticipant> Participants { get; set; } = [];
    public ICollection<ChatMedia> Medias { get; set; } = [];
}
