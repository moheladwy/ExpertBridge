using ExpertBridge.Core.DTOs.Responses;
using ExpertBridge.Core.Entities.Post;

namespace ExpertBridge.Core.Interfaces.Services;

public interface IPostService
{
    Task<PostResponse> GetByIdAsync(string id);
}
