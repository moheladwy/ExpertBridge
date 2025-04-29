

namespace ExpertBridge.Core.Entities.Media.MediaGrants
{
    public class MediaGrant : ISoftDeletable
    {
        public int Id { get; set; }
        public required string Key { get; set; }
        public bool OnHold { get; set; }
        public bool IsActive { get; set; }
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ActivatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
