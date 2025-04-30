

namespace ExpertBridge.Core.Entities
{
    public abstract class BaseModel : ITimestamped
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime? CreatedAt { get; set; } 
        public DateTime? LastModified { get; set; }
    }
}
