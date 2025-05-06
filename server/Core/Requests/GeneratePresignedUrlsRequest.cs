

using Core.Entities;

namespace Core.Requests
{
    public class GeneratePresignedUrlsRequest
    {
        public List<FileMetadata> Files { get; set; }
    }
}
