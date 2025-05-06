

using Core.Responses;

namespace Core.Interfaces.Services;

public interface IPostsService
{
    Task<PostResponse> GetByIdAsync(string id);
}
