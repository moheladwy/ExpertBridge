namespace ExpertBridge.Api.Core.Entities.Media.ChatMedia;

public class ChatMedia
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Foreign keys
    public string ChatId { get; set; }
    public string MediaId { get; set; }

    // Navigation properties
    public Chat.Chat Chat { get; set; }
    public Media Media { get; set; }
}
