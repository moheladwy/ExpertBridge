using ExpertBridge.Core.Responses;

namespace ExpertBridge.Core.Interfaces.Services;

public interface IPostsService
{
    Task<PostResponse> GetByIdAsync(string id);
}
