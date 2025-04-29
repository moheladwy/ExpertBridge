

using ExpertBridge.Core.Entities;

namespace ExpertBridge.Core.Requests
{
    public class GeneratePresignedUrlsRequest
    {
        public List<FileMetadata> Files { get; set; }
    }
}
