namespace Core.Entities.Media.ChatMedia;

public class ChatMedia : MediaObject
{
    // Foreign keys
    public string ChatId { get; set; }
    // Navigation properties
    public Chats.Chat Chat { get; set; }
}
