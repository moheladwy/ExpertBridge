

namespace ExpertBridge.Core.Entities
{
    public abstract class BaseModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModified { get; set; }
    }
}
