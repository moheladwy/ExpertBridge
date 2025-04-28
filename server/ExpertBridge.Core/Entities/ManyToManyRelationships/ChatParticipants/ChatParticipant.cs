

using ExpertBridge.Core.Entities.Chats;
using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ChatParticipants;

public class ChatParticipant
{
    public string ChatId { get; set; }
    public string ProfileId { get; set; }

    // Navigation properties
    public Chat Chat { get; set; }
    public Profile Profile { get; set; }
}
