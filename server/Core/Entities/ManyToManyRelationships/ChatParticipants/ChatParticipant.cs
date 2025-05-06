using Core.Entities.Chats;
using Core.Entities.Profiles;

namespace Core.Entities.ManyToManyRelationships.ChatParticipants;
public class ChatParticipant
{
    public string ChatId { get; set; }
    public string ProfileId { get; set; }

    // Navigation properties
    public Chat Chat { get; set; }
    public Profile Profile { get; set; }
}
