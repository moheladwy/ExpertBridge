namespace ExpertBridge.Core.Entities.Chat;

public class Chat
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    // Navigation properties
    public ICollection<ChatParticipant.ChatParticipant> Participants { get; set; } = [];
    public ICollection<Media.Media> Medias { get; set; } = [];
}
