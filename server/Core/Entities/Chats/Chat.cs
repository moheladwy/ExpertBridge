

using Core.Entities.ManyToManyRelationships.ChatParticipants;
using Core.Entities.Media.ChatMedia;

namespace Core.Entities.Chats;

public class Chat : BaseModel, ISoftDeletable
{
    public DateTime? EndedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public ICollection<ChatParticipant> Participants { get; set; } = [];
    public ICollection<ChatMedia> Medias { get; set; } = [];
}
