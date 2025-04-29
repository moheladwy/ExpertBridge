

namespace ExpertBridge.Core.Entities
{
    public class FileMetadata
    {
        public string ContentType { get; set; }
        public string? Key { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string Extension { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
