namespace ExpertBridge.Core.Entities.Chat.ChatParticipant;

public class ChatParticipant
{
    public string ChatId { get; set; }
    public string ProfileId { get; set; }

    // Navigation properties
    public Chat Chat { get; set; }
    public Profile.Profile Profile { get; set; }
}
